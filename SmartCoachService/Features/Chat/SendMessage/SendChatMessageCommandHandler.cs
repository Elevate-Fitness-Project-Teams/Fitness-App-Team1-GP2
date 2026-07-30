using MediatR;
using SmartCoachService.Common.Abstractions;
using SmartCoachService.Common.Exceptions;
using SmartCoachService.Domain.Entities;
using SmartCoachService.Domain.Enums;
using SmartCoachService.Infrastructure.ExternalServices;
using SmartCoachService.Infrastructure.Messaging.Events;

namespace SmartCoachService.Features.Chat.SendMessage;

/// <summary>
/// Free tier is capped at 5 messages / 24h via a cache counter (PERM_PREMIUM_REQUIRED once exceeded).
/// Pulls context from FCE + Progress, feeds it to the AI prompt, and persists the User/AI turn pair.
/// </summary>
public sealed class SendChatMessageCommandHandler : IRequestHandler<SendChatMessageCommand, SendChatMessageResult>
{
    private const int FreeTierDailyMessageLimit = 5;

    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly ICacheService _cache;
    private readonly IFceServiceClient _fceServiceClient;
    private readonly IProgressServiceClient _progressServiceClient;
    private readonly IAiCoachClient _aiCoachClient;
    private readonly IRabbitMqPublisher _publisher;

    public SendChatMessageCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        ICacheService cache,
        IFceServiceClient fceServiceClient,
        IProgressServiceClient progressServiceClient,
        IAiCoachClient aiCoachClient,
        IRabbitMqPublisher publisher)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _cache = cache;
        _fceServiceClient = fceServiceClient;
        _progressServiceClient = progressServiceClient;
        _aiCoachClient = aiCoachClient;
        _publisher = publisher;
    }

    public async Task<SendChatMessageResult> Handle(SendChatMessageCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsPremium)
        {
            var rateLimitKey = $"smart-coach:messages:{_currentUser.UserId:N}:{DateTime.UtcNow:yyyyMMdd}";
            var messagesSentToday = await _cache.IncrementAsync(rateLimitKey, TimeSpan.FromHours(24), cancellationToken);
            if (messagesSentToday > FreeTierDailyMessageLimit)
                throw new ForbiddenException("PERM_PREMIUM_REQUIRED", "Free tier is limited to 5 messages every 24 hours.");
        }

        ChatSession session;
        if (request.SessionId.HasValue)
        {
            session = await _unitOfWork.ChatSessions.GetByIdAsync(request.SessionId.Value, cancellationToken)
                ?? throw new NotFoundException("RES_SESSION_NOT_FOUND", $"Chat session {request.SessionId} was not found.");
        }
        else
        {
            session = new ChatSession { UserId = _currentUser.UserId };
            await _unitOfWork.ChatSessions.AddAsync(session, cancellationToken);
        }

        // Build AI context from FCE (calorie target) + Progress (streak/weight trend).
        var fce = await _fceServiceClient.GetCalorieTargetAsync(_currentUser.UserId, cancellationToken);
        var progress = await _progressServiceClient.GetProgressSummaryAsync(_currentUser.UserId, cancellationToken);

        var contextPrompt =
            $"User daily calorie goal: {(fce?.IsCalculated == true ? fce.DailyGoalCalories.ToString() : "not calculated yet")}. " +
            $"Current streak: {progress?.StreakDays ?? 0} day(s), weight change: {progress?.WeightDeltaKg ?? 0}kg.";

        var aiReply = await _aiCoachClient.GetReplyAsync(request.Message, contextPrompt, cancellationToken);

        session.LastActivityAt = DateTime.UtcNow;
        _unitOfWork.ChatSessions.Update(session);

        await _unitOfWork.ChatMessages.AddAsync(new ChatMessage
        {
            SessionId = session.Id,
            Sender = ChatSender.User,
            Content = request.Message
        }, cancellationToken);

        await _unitOfWork.ChatMessages.AddAsync(new ChatMessage
        {
            SessionId = session.Id,
            Sender = ChatSender.AI,
            Content = aiReply.Message,
            FollowUpSuggestions = aiReply.FollowUpSuggestions
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _publisher.PublishAsync(
            new ChatMessageSentEvent(_currentUser.UserId, session.Id, DateTime.UtcNow),
            routingKey: "smart-coach.chat.message-sent",
            cancellationToken);

        return new SendChatMessageResult
        {
            SessionId = session.Id,
            Reply = aiReply.Message,
            FollowUpSuggestions = aiReply.FollowUpSuggestions
        };
    }
}
