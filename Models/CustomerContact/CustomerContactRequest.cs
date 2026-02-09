using System.ComponentModel.DataAnnotations;

namespace SimpleCRM.Models.CustomerContact
{
    /// <summary>
    /// CustomerContact request model for Insert/Update operations
    /// Used with ssp_tblCustomerContact_InsertUpdate stored procedure
    /// </summary>
    public class CustomerContactRequest
    {
        /// <summary>
        /// CustomerContact Id - 0 for Insert, >0 for Update
        /// </summary>
        public int Id { get; set; }

        [Required(ErrorMessage = "Customer Id is required")]
        public int CustomerId { get; set; }

        [StringLength(100, ErrorMessage = "Mobile number cannot exceed 100 characters")]
        public string? MobileNo { get; set; }

        [StringLength(100, ErrorMessage = "Address cannot exceed 100 characters")]
        public string? Address { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string? Email { get; set; }

        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
        public string? City { get; set; }

        [StringLength(100, ErrorMessage = "State cannot exceed 100 characters")]
        public string? State { get; set; }

        [StringLength(100, ErrorMessage = "Phone number cannot exceed 100 characters")]
        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; } = true;

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }
    }
}
