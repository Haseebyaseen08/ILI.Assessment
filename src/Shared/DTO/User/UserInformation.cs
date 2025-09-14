using Shared.Enums;
using System.Text.Json.Serialization;

namespace Shared.DTO.User
{
    public record UserInformation
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public SubscriptionType SubscriptionType { get; set; }

        [JsonIgnore]
        public string Password { get; set; } = string.Empty;
    }
}
