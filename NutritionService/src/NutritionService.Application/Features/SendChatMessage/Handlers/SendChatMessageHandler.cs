using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NutritionService.Application.Common.Exceptions;
using NutritionService.Domain.Common.Interfaces;
using SmartCoachService.Application.Common.Interfaces;
using SmartCoachService.Application.Features.SendChatMessage.Commands;
using SmartCoachService.Application.Features.SendChatMessage.Dtos;
using SmartCoachService.Application.Features.SendChatMessage.Validators;
using SmartCoachService.Domain.Entities;
using SmartCoachService.Domain.Enums;

namespace SmartCoachService.Application.Features.SendChatMessage.Handlers;

public sealed class SendChatMessageHandler
    : IRequestHandler<SendChatMessageCommand, SendChatMessageResponse>
{
    // Free-tier cap: 5 messages per 24 hours (checked via distributed cache).
    private const int FreeTierDailyLimit = 5;

    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;
    private readonly IAiCoachService _aiCoach;
    private readonly IDistributedCache _cache;

    public SendChatMessageHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser,
        IAiCoachService aiCoach,
        IDistributedCache cache)
    {
        _uow = uow;
        _currentUser = currentUser;
        _aiCoach = aiCoach;
        _cache = cache;
    }

    public async Task<SendChatMessageResponse> Handle(
        SendChatMessageCommand request, CancellationToken cancellationToken)
    {
        SendChatMessageValidator.EnsureValid(request);

        var userId = _currentUser.UserId;

        // ── 1. Free-tier rate limit ──────────────────────────────────────────────
        // Premium users bypass the cap entirely.
        if (!_currentUser.IsPremium)
        {
            var rateLimitKey = $"smart-coach:msg-count:{userId}:{DateTime.UtcNow:yyyyMMdd}";
            var countRaw = await _cache.GetStringAsync(rateLimitKey, cancellationToken);
            var count = int.TryParse(countRaw, out var n) ? n : 0;

            if (count >= FreeTierDailyLimit)
                throw new PremiumRequiredException();

            // Increment counter; expire at end of the UTC day.
            var midnight = DateTime.UtcNow.Date.AddDays(1);
            var ttl = midnight - DateTime.UtcNow;
            await _cache.SetStringAsync(rateLimitKey, (count + 1).ToString(),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl },
                cancellationToken);
        }

        // ── 2. Resolve or create session ────────────────────────────────────────
        ChatSession session;

        if (request.SessionId.HasValue)
        {
            session = await _uow.Repository<ChatSession>()
                .Query()
                .SingleOrDefaultAsync(s => s.Id == request.SessionId.Value && s.UserId == userId,
                    cancellationToken)
                ?? throw new SessionNotFoundException(request.SessionId.Value);
        }
        else
        {
            // No sessionId supplied → start a brand-new session
            session = new ChatSession
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CreatedAtUtc = DateTime.UtcNow,
                LastActivityAtUtc = DateTime.UtcNow
            };
            await _uow.Repository<ChatSession>().AddAsync(session, cancellationToken);
        }

        // ── 3. Load user context from local read models ──────────────────────────
        var fceCtx = await _uow.Repository<UserFceContext>()
            .Query()
            .AsNoTracking()
            .SingleOrDefaultAsync(f => f.UserId == userId, cancellationToken);

        var progressCtx = await _uow.Repository<UserProgressContext>()
            .Query()
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.UserId == userId, cancellationToken);

        var userContext = new UserContext(
            DailyGoalCalories: fceCtx?.DailyGoalCalories ?? 0,
            MinProteinGrams: fceCtx?.MinProteinGrams ?? 0,
            FitnessGoal: fceCtx?.FitnessGoal,
            CurrentWeightKg: progressCtx?.CurrentWeightKg,
            BodyFatPercentage: progressCtx?.BodyFatPercentage,
            CompletedWorkoutsLast30Days: progressCtx?.CompletedWorkoutsLast30Days ?? 0);

        // ── 4. Build conversation history (last 20 messages for context window) ──
        var history = await _uow.Repository<ChatMessage>()
            .Query()
            .AsNoTracking()
            .Where(m => m.SessionId == session.Id)
            .OrderByDescending(m => m.SentAtUtc)
            .Take(20)
            .OrderBy(m => m.SentAtUtc)
            .Select(m => $"{m.Sender}: {m.Content}")
            .ToListAsync(cancellationToken);

        // ── 5. Call AI service ───────────────────────────────────────────────────
        var aiReply = await _aiCoach.GetReplyAsync(request.Message, userContext, history, cancellationToken);

        // ── 6. Persist: User message + AI message inside one transaction ─────────
        var userMsgId = Guid.NewGuid();
        var aiMsgId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        await _uow.ExecuteInTransactionAsync(async () =>
        {
            session.LastActivityAtUtc = now;
            _uow.Repository<ChatSession>().Update(session);

            await _uow.Repository<ChatMessage>().AddRangeAsync(new[]
            {
                new ChatMessage { Id = userMsgId, SessionId = session.Id, Sender = SenderType.User,
                    Content = request.Message, SentAtUtc = now },
                new ChatMessage { Id = aiMsgId,   SessionId = session.Id, Sender = SenderType.AI,
                    Content = aiReply.Message,    SentAtUtc = now.AddMilliseconds(1) }
            }, cancellationToken);
        }, cancellationToken);

        return new SendChatMessageResponse(
            session.Id,
            aiMsgId,
            aiReply.Message,
            aiReply.FollowUpSuggestions);
    }
}
