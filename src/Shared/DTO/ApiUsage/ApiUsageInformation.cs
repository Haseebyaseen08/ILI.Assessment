namespace Shared.DTO.ApiUsage
{
    public record ApiUsageInformation
    {
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public string Endpoint { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
