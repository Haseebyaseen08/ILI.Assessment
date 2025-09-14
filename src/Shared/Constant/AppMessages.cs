namespace Shared.Constant
{
    public static class AppMessages
    {
        public const string InternalServerError = "An unexpected error occurred. Please try again later.";
        public const string NotFound = "The requested resource was not found.";
        public const string BadRequest = "The request was invalid or cannot be served.";
        public const string Unauthorized = "You are not authorized to perform this action.";
        public const string Forbidden = "You do not have permission to access this resource.";
        public const string Conflict = "A conflict occurred with the current state of the resource.";
        public const string CreatedSuccessfully = "The resource was created successfully.";
        public const string UpdatedSuccessfully = "The resource was updated successfully.";
        public const string DeletedSuccessfully = "The resource was deleted successfully.";
        public const string InvalidCredentials = "Invalid email or password.";
        
        // Rate Limiting Messages
        public const string RateLimitExceeded = "Rate limit exceeded. Please try again later.";
        public const string MonthlyQuotaExceeded = "Monthly quota exceeded. Please upgrade your plan or wait for the next month.";
        public const string TierNotFound = "Subscription tier not found.";
    }
}
