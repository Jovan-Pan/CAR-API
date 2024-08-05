namespace Entities;

public class ApiResponse<T>
{
    public bool Success { get; set; } = true;
    public string Message { get; set; } = "Success";
    public T? Content { get; set; }

    public static ApiResponse<T> FailResponse(string message)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message
        };
    }

    public static ApiResponse<T> FailResponse(string message, T content)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Content = content
        };
    }

    public static ApiResponse<T> SuccessResponse(T? content, string message = "Success")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Content = content
        };
    }
}
