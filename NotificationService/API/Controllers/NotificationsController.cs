using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.Common;
using NotificationService.Application.Notifications.GetNotifications;
using NotificationService.Application.Notifications.MarkNotificationAsRead;
using NotificationService.Common;
using System.Security.Claims;

namespace NotificationService.API.Controllers;

[ApiController]
[Route("api/v1/notifications")]
public sealed class NotificationsController(
    IMediator mediator,
    IValidator<GetNotificationsQuery> getNotificationsValidator,
    IValidator<MarkNotificationAsReadCommand> markNotificationAsReadValidator)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetNotifications(CancellationToken cancellationToken)
    {
        var userIdValue = "10";
        //User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        //?? User.FindFirst("sub")?.Value
        //?? User.FindFirst("userId")?.Value;

        if (!int.TryParse(userIdValue, out var userId))
        {
            var unauthorizedResponse =
                ApiResponse<List<GetNotificationsResponse>>.Failure(ErrorCode.Unauthorized);

            return Unauthorized(unauthorizedResponse);
        }

        var query = new GetNotificationsQuery(userId);

        var validationResult = await getNotificationsValidator.ValidateAsync(
            query,
            cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(x => x.ErrorMessage)
                .ToList();

            var validationResponse =
                ApiResponse<List<GetNotificationsResponse>>
                    .Failure(ErrorCode.ValidationError, errors);

            return BadRequest(validationResponse);
        }

        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            var failureResponse =
                ApiResponse<List<GetNotificationsResponse>>.Failure(result.Error);

            return StatusCode(failureResponse.StatusCode, failureResponse);
        }

        var response = ApiResponse<List<GetNotificationsResponse>>.Success(
            result.Data,
            "Notifications fetched successfully.");

        return Ok(response);
    }

    [HttpPut("{id:int}/read")]
    public async Task<IActionResult> MarkAsRead(
    int id,
    CancellationToken cancellationToken)
    {
        
        var userIdValue =/*"10";*/
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value
       ?? User.FindFirst("sub")?.Value
       ?? User.FindFirst("userId")?.Value;

        if (!int.TryParse(userIdValue, out var userId))
        {
            var unauthorizedResponse =
                ApiResponse<MarkNotificationAsReadResponse>.Failure(ErrorCode.Unauthorized);

            return Unauthorized(unauthorizedResponse);
        }

        var command = new MarkNotificationAsReadCommand(id, userId);

        var validationResult = await markNotificationAsReadValidator.ValidateAsync(
            command,
            cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(x => x.ErrorMessage)
                .ToList();

            var validationResponse =
                ApiResponse<MarkNotificationAsReadResponse>
                    .Failure(ErrorCode.ValidationError, errors);

            return BadRequest(validationResponse);
        }

        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            var failureResponse =
                ApiResponse<MarkNotificationAsReadResponse>.Failure(result.Error);

            return StatusCode(failureResponse.StatusCode, failureResponse);
        }

        var response = ApiResponse<MarkNotificationAsReadResponse>.Success(
            result.Data,
            "Notification marked as read successfully.");

        return Ok(response);
    }
}