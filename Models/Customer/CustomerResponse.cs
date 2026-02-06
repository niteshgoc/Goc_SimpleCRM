namespace SimpleCRM.Models.Customer
{
    /// <summary>
    /// Customer response model for API/View responses
    /// Includes all customer data
    /// </summary>
    public class CustomerResponse
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public string Status => IsActive ? "Active" : "Inactive";
    }
}
