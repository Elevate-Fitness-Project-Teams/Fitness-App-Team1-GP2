namespace WorkoutService.Common
{
    public sealed record ApiResponse<T>(
       bool IsSuccess,
       string Message,
       T? Data,
       List<string> Errors,
       int StatusCode,
       DateTime Timestamp
   )
    {
        public static ApiResponse<T> Success(
               T? data,
               string message = "Success",
               int statusCode = StatusCodes.Status200OK)
        {
            return new ApiResponse<T>(
                IsSuccess: true,
                Message: message,
                Data: data,
                Errors: new List<string>(),
                StatusCode: statusCode,
                Timestamp: DateTime.UtcNow
            );
        }
        public static ApiResponse<T> Failure(
            ErrorCode error,
            List<string>? errors = null)
        {
            var statusCode = GetStatusCode(error);

            return new ApiResponse<T>(
                false,
                GetMessage(error),
                default,
                errors ?? new List<string> { error.ToString() },
                statusCode,
                DateTime.UtcNow
            );
        }

        private static int GetStatusCode(ErrorCode error)
        {
            return error switch
            {
                ErrorCode.WorkoutNotFound => StatusCodes.Status404NotFound,
                ErrorCode.WorkoutPlanNotFound => StatusCodes.Status404NotFound,
                ErrorCode.ExerciseNotFound => StatusCodes.Status404NotFound,
                ErrorCode.WorkoutSessionNotFound => StatusCodes.Status404NotFound,

                ErrorCode.TokenInvalid => StatusCodes.Status401Unauthorized,
                ErrorCode.TokenExpired => StatusCodes.Status401Unauthorized,
                ErrorCode.Unauthorized => StatusCodes.Status401Unauthorized,

                ErrorCode.Forbidden => StatusCodes.Status403Forbidden,

                ErrorCode.RateLimitExceeded => StatusCodes.Status429TooManyRequests,

                ErrorCode.ServiceUnavailable => StatusCodes.Status503ServiceUnavailable,

                ErrorCode.DatabaseError => StatusCodes.Status500InternalServerError,
                ErrorCode.SaveChangesFailed => StatusCodes.Status500InternalServerError,
                ErrorCode.ServerError => StatusCodes.Status500InternalServerError,

                _ => StatusCodes.Status400BadRequest
            };
        }

        private static string GetMessage(ErrorCode error)
        {
            return error switch
            {
                ErrorCode.WorkoutNotFound => "Workout was not found.",
                ErrorCode.WorkoutPlanNotFound => "Workout plan was not found.",
                ErrorCode.ExerciseNotFound => "Exercise was not found.",
                ErrorCode.WorkoutSessionNotFound => "Workout session was not found.",

                ErrorCode.InvalidWorkoutId => "Invalid workout id.",
                ErrorCode.InvalidWorkoutCategory => "Invalid workout category.",
                ErrorCode.InvalidWorkoutDifficulty => "Invalid workout difficulty.",
                ErrorCode.InvalidWorkoutDuration => "Invalid workout duration.",

                ErrorCode.InvalidWorkoutPlanId => "Invalid workout plan id.",

                ErrorCode.InvalidExerciseId => "Invalid exercise id.",

                ErrorCode.InvalidSessionId => "Invalid session id.",
                ErrorCode.InvalidSessionStatus => "Invalid session status.",

                ErrorCode.InvalidPage => "Invalid page.",
                ErrorCode.InvalidPageSize => "Invalid page size.",

                ErrorCode.TokenInvalid => "Invalid token.",
                ErrorCode.TokenExpired => "Token expired.",
                ErrorCode.Unauthorized => "Unauthorized.",
                ErrorCode.Forbidden => "Forbidden.",
                ErrorCode.RateLimitExceeded => "Rate limit exceeded.",

                ErrorCode.DatabaseError => "Database error.",
                ErrorCode.SaveChangesFailed => "Failed to save changes.",
                ErrorCode.ServerError => "Server error.",
                ErrorCode.ServiceUnavailable => "Service unavailable.",

                _ => "Request failed."
            };
        }

    }
}
