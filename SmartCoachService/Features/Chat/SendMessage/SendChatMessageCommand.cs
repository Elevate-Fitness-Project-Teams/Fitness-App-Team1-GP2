using MediatR;

namespace SmartCoachService.Features.Chat.SendMessage;

/// <summary>CQRS command for User Story 7.1 — POST /api/v1/smart-coach/chat.</summary>
public sealed record SendChatMessageCommand(string Message, Guid? SessionId) : IRequest<SendChatMessageResult>;
