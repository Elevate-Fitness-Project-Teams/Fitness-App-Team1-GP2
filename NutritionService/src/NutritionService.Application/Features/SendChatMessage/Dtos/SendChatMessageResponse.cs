namespace SmartCoachService.Application.Features.SendChatMessage.Dtos;

public sealed record SendChatMessageResponse(
    Guid SessionId,
    Guid MessageId,
    string Reply,
    IReadOnlyList<string> FollowUpSuggestions);
