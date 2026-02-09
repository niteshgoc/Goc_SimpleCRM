namespace SimpleCRM.Models.CustomerContact
{
    /// <summary>
    /// CustomerContact entity model - maps to tblCustomerContact table
    /// Stores customer contact information including phone, email, and address details
    /// </summary>
    public class CustomerContact
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string? MobileNo { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? City { get; set; }
        public int State { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
