using NutritionService.Application.Common.Exceptions;
using SmartCoachService.Application.Features.SendChatMessage.Commands;

namespace SmartCoachService.Application.Features.SendChatMessage.Validators;

public static class SendChatMessageValidator
{
    public static void EnsureValid(SendChatMessageCommand cmd)
    {
        if (string.IsNullOrWhiteSpace(cmd.Message))
            throw new ValidationAppException("VAL_MESSAGE_REQUIRED", "message cannot be empty.");

        if (cmd.Message.Length > 2000)
            throw new ValidationAppException("VAL_MESSAGE_TOO_LONG", "message must be 2000 characters or fewer.");
    }
}
