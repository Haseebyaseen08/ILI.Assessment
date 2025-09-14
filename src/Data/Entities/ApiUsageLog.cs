namespace Data.Entities
{
    public class ApiUsageLog
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public string Endpoint { get; set; } = "";
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Navigation property
        public Customer? Customer { get; set; }
        public User? User { get; set; }
    }

}
