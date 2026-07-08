namespace WorkoutService.Common
{
    public enum ErrorCode
    {
        None = 0,

        // General validation errors
        ValidationError = 100,
        RequiredField = 101,
        InvalidId = 102,
        InvalidPage = 103,
        InvalidPageSize = 104,

        // Auth / Gateway errors
        TokenInvalid = 200,
        TokenExpired = 201,
        Unauthorized = 202,
        Forbidden = 203,
        RateLimitExceeded = 204,

        // Workout errors
        WorkoutNotFound = 300,
        InvalidWorkoutId = 301,
        InvalidWorkoutCategory = 302,
        InvalidWorkoutDifficulty = 303,
        InvalidWorkoutDuration = 304,

        // Workout plan errors
        WorkoutPlanNotFound = 400,
        InvalidWorkoutPlanId = 401,

        // Exercise errors
        ExerciseNotFound = 500,
        InvalidExerciseId = 501,

        // Workout session errors
        WorkoutSessionNotFound = 600,
        InvalidSessionId = 601,
        InvalidSessionStatus = 602,
        WorkoutSessionDoesNotBelongToUser = 603,

        // Server errors
        DatabaseError = 900,
        SaveChangesFailed = 901,
        ServerError = 902,
        ServiceUnavailable = 903
    }
}