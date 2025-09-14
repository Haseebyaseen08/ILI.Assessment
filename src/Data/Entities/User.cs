namespace Data.Entities
{
    public class User
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public int CustomerId { get; set; } // Foreign key to Customer

        // Navigation property
        public Customer? Customer { get; set; }

        public ICollection<ApiUsageLog> ApiUsageLogs { get; set; } = new List<ApiUsageLog>();

    }
}
