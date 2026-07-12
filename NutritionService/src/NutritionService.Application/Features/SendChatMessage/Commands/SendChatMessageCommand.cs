using MediatR;
using SmartCoachService.Application.Features.SendChatMessage.Dtos;

namespace SmartCoachService.Application.Features.SendChatMessage.Commands;

public sealed record SendChatMessageCommand(
    string Message,
    Guid? SessionId) : IRequest<SendChatMessageResponse>;
