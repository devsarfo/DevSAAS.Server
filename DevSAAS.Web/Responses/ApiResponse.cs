using System.Collections;
using Microsoft.AspNetCore.Mvc;

namespace DevSAAS.Web.Responses;

public class ApiResponse
{
    public string Status { get; }
    public string Message { get; }

    public ApiResponse(string status, string message)
    {
        Status = status;
        Message = message;
    }

    public static IActionResult Send(int code, string status, string message) 
        => new ObjectResult(new ApiResponse(status, message)) { StatusCode = code };

    public static IActionResult Send<TRecord>(int code, string status, string message, TRecord data) 
        => new ObjectResult(new ApiResponse<TRecord>(status, message, data)) { StatusCode = code };
}

public sealed class ApiResponse<TRecord> : ApiResponse
{
    public ApiResponse(string status, string message, TRecord? data) : base(status, message)
    {
        Data = data;
    }
    
    public TRecord? Data { get; }
}