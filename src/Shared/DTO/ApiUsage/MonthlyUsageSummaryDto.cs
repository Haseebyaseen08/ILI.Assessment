using Shared.Enums;

namespace Shared.DTO.ApiUsage
{
    public class MonthlyUsageSummaryDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = "";
        public string CompanyName { get; set; } = "";
        public int Year { get; set; }
        public int Month { get; set; }
        public SubscriptionType SubscriptionType { get; set; }
        public int TotalApiCalls { get; set; }
        public decimal PricePerCall { get; set; }
        public decimal TotalCost { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Dictionary<string, int> EndpointUsage { get; set; } = new();
    }
}