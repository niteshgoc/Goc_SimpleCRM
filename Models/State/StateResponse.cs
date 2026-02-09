namespace SimpleCRM.Models.State
{
    /// <summary>
    /// State response model - for API/View responses
    /// </summary>
    public class StateResponse
    {
        public int Id { get; set; }
        public string StateName { get; set; } = string.Empty;
        public string? StateCode { get; set; }
        public bool IsActive { get; set; }
    }
}
