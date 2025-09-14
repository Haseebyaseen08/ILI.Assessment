using System.Security.Claims;

namespace Shared.JWT
{
    public interface IJWTService
    {
        string GenerateToken(int userId, int customerId, string customerSubscriptionPlan, string email, string firstName, string lastName);
        ClaimsPrincipal? ValidateToken(string token);
    }
}
