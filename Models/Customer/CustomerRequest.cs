using System.ComponentModel.DataAnnotations;

namespace SimpleCRM.Models.Customer
{
    /// <summary>
    /// Customer request model for Insert/Update operations
    /// Used with ssp_Customer_InsertUpdate stored procedure
    /// </summary>
    public class CustomerRequest
    {
        /// <summary>
        /// Customer Id - 0 for Insert, >0 for Update
        /// </summary>
        public int Id { get; set; }

        [Required(ErrorMessage = "Company name is required")]
        [StringLength(200, ErrorMessage = "Company name cannot exceed 200 characters")]
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone is required")]
        [StringLength(50, ErrorMessage = "Phone cannot exceed 50 characters")]
        public string Phone { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        public string? Address { get; set; }

        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
        public string? City { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
