using Shared.Enums;

namespace Data.Entities
{
    public class MonthlyUsageSummary
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public SubscriptionType SubscriptionType { get; set; }
        public int TotalApiCalls { get; set; }
        public decimal PricePerCall { get; set; }
        public decimal TotalCost { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Customer? Customer { get; set; }

        // Dictionary to store endpoint usage breakdown (serialized as JSON)
        public string EndpointUsageJson { get; set; } = "{}";
    }
}