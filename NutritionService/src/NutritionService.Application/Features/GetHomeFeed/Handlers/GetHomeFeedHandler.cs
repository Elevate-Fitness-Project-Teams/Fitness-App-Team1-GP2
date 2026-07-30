using MediatR;
using Microsoft.EntityFrameworkCore;
using NutritionService.Application.Common.Exceptions;
using NutritionService.Domain.Common.Interfaces;
using SmartCoachService.Application.Common.Interfaces;
using SmartCoachService.Application.Features.GetHomeFeed.Dtos;
using SmartCoachService.Application.Features.GetHomeFeed.Queries;
using SmartCoachService.Domain.Entities;
using System.Text.Json;

namespace SmartCoachService.Application.Features.GetHomeFeed.Handlers;

public sealed class GetHomeFeedHandler : IRequestHandler<GetHomeFeedQuery, HomeFeedDto>
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(30);

    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;
    private readonly IHomeFeedAggregator _aggregator;

    public GetHomeFeedHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser,
        IHomeFeedAggregator aggregator)
    {
        _uow = uow;
        _currentUser = currentUser;
        _aggregator = aggregator;
    }

    public async Task<HomeFeedDto> Handle(GetHomeFeedQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        // ── 1. Cache-first: check RecommendationCache ─────────────────────────────
        var cached = await _uow.Repository<RecommendationCache>()
            .Query()
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (cached is not null && !cached.IsExpired)
        {
            // Cache hit: deserialize and return directly
            var cachedPayload = JsonSerializer.Deserialize<HomeFeedDto>(cached.HomeFeedDataJson)!;
            return cachedPayload with { FromCache = true };
        }

        // ── 2. Cache miss or expired: aggregate from downstream services ───────────
        HomeFeedPayload freshPayload;
        try
        {
            freshPayload = await _aggregator.AggregateAsync(userId, cancellationToken);
        }
        catch (ServiceUnavailableException)
        {
            throw; // propagate 503 as-is
        }
        catch (Exception ex)
        {
            // Any unexpected downstream failure also becomes 503
            throw new ServiceUnavailableException($"Aggregation failed: {ex.Message}");
        }

        var now = DateTime.UtcNow;
        var dto = new HomeFeedDto(
            freshPayload.ProfileSummary,
            freshPayload.FceMetrics,
            freshPayload.WorkoutSuggestions,
            freshPayload.MealRecommendations,
            freshPayload.ProgressSnapshot,
            FromCache: false,
            GeneratedAtUtc: now);

        // ── 3. Upsert RecommendationCache ─────────────────────────────────────────
        var json = JsonSerializer.Serialize(dto);

        if (cached is null)
        {
            await _uow.Repository<RecommendationCache>().AddAsync(new RecommendationCache
            {
                UserId = userId,
                HomeFeedDataJson = json,
                CreatedAtUtc = now,
                ExpiresAt = now.Add(CacheTtl)
            }, cancellationToken);
        }
        else
        {
            cached.HomeFeedDataJson = json;
            cached.CreatedAtUtc = now;
            cached.ExpiresAt = now.Add(CacheTtl);
            _uow.Repository<RecommendationCache>().Update(cached);
        }

        await _uow.SaveChangesAsync(cancellationToken);

        return dto;
    }
}
