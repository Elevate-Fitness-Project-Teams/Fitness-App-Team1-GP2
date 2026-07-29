using MediatR;

namespace SmartCoachService.Features.HomeFeed.GetHomeFeed;

/// <summary>CQRS query for User Story 7.3 — GET /api/v1/home.</summary>
public sealed record GetHomeFeedQuery : IRequest<HomeFeedDto>;
