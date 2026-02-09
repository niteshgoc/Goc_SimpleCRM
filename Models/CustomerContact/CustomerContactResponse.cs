namespace SimpleCRM.Models.CustomerContact
{
    /// <summary>
    /// CustomerContact response model for API/View responses
    /// Includes all customer contact data with optional customer details
    /// </summary>
    public class CustomerContactResponse
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string? MobileNo { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Customer details from JOIN
        public string? CustomerName { get; set; }
        public string? CustomerCode { get; set; }

        // Computed properties
        public string Status => IsActive ? "Active" : "Inactive";
        public string FullAddress => !string.IsNullOrEmpty(City) && !string.IsNullOrEmpty(State)
            ? $"{Address}, {City}, {State}"
            : Address ?? string.Empty;
    }
}
