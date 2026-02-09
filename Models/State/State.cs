namespace SimpleCRM.Models.State
{
    /// <summary>
    /// State entity model - maps to tblState table
    /// Stores state master data
    /// </summary>
    public class State
    {
        public int Id { get; set; }
        public string StateName { get; set; } = string.Empty;
        public string? StateCode { get; set; }
        public bool IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
