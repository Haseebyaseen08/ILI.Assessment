namespace Shared.Settings
{
    public class RateLimitOptions
    {
        public Dictionary<string, RateLimitConfig> Plans { get; set; } = new();
    }

    public class RateLimitConfig
    {
        public int RequestsPerSecond { get; set; }
        public int MonthlyQuota { get; set; }
        public PricePerMonth PricePerMonth { get; set; } = new();
    }

    public class PricePerMonth
    {
        public decimal PricingPerMonth { get; set; }
        public string Currency { get; set; } = "USD";
        public decimal PricePerCall { get; set; }
    }
}
