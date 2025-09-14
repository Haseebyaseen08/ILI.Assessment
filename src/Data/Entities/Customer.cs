using Shared.Enums;

namespace Data.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public required string CustomerName { get; set; }
        public required string CompanyName { get; set; }
        public required string ContactEmail { get; set; }
        public required string ContactPhone { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public SubscriptionType SubscriptionType { get; set; }

        // Navigation property
        public ICollection<User> Users { get; set; } = new List<User>();

        public ICollection<ApiUsageLog> ApiUsageLogs { get; set; } = new List<ApiUsageLog>();
    }
}