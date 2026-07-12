namespace ProgressTrackingService.Common.Response;

public enum StatusCode
{
    Success = 200,
    BadRequest = 400,
    NotFound = 404,
    Unauthorized = 401,
    InternalServerError = 500,
}