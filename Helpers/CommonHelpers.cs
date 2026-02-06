using System.Text.Json;

namespace SimpleCRM.Helpers
{
    public static class CommonHelpers
    {
        /// <summary>
        /// Serialize object to JSON string for JavaScript
        /// </summary>
        public static string SerializeObject(object obj)
        {
            if (obj == null)
                return "null";

            return JsonSerializer.Serialize(obj, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            });
        }
    }
}
