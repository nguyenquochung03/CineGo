namespace CineGo.Models
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public int Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }

        public static ApiResponse SuccessResponse(object? data = null, string message = "Thành công")
        {
            return new ApiResponse
            {
                Success = true,
                Status = 200,
                Message = message,
                Data = data
            };
        }

        public static ApiResponse ErrorResponse(int status, string message)
        {
            return new ApiResponse
            {
                Success = false,
                Status = status,
                Message = message
            };
        }

        public static ApiResponse ErrorResponse(int status, string message, object? data = null)
        {
            return new ApiResponse
            {
                Success = false,
                Status = status,
                Message = message,
                Data = data
            };
        }

    }
}
