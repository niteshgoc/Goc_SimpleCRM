namespace SimpleCRM.Models
{
    /// <summary>
    /// Generic service response wrapper for repository operations
    /// Non-generic version for operations without specific data return
    /// </summary>
    public class ServiceResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }
    }

    /// <summary>
    /// Generic service response wrapper for repository operations
    /// Generic version with strongly-typed data
    /// </summary>
    /// <typeparam name="T">Type of data being returned</typeparam>
    public class ServiceResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        /// <summary>
        /// Creates a successful response
        /// </summary>
        public static ServiceResponse<T> Success(T data, string message = "Operation successful")
        {
            return new ServiceResponse<T>
            {
                IsSuccess = true,
                Message = message,
                Data = data
            };
        }

        /// <summary>
        /// Creates a failed response
        /// </summary>
        public static ServiceResponse<T> Failure(string message, T? data = default)
        {
            return new ServiceResponse<T>
            {
                IsSuccess = false,
                Message = message,
                Data = data
            };
        }
    }
}
