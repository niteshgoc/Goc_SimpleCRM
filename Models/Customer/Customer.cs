namespace SimpleCRM.Models.Customer
{
    /// <summary>
    /// Customer entity model - maps to Customer table
    /// </summary>
    public class Customer
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
