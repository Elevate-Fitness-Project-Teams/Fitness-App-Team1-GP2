using MediatR;
using SmartCoachService.Application.Features.GetHomeFeed.Dtos;

namespace SmartCoachService.Application.Features.GetHomeFeed.Queries;

public sealed record GetHomeFeedQuery : IRequest<HomeFeedDto>;
