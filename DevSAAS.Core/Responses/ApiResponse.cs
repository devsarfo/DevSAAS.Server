using System.Collections;

namespace DevSAAS.Core.Responses;

public class ApiResponse<TRecord>
{
    public string Status { get; set; }
    public string Message { get; set; }
    public TRecord? Data { get; set; }

    public ApiResponse(string status, string message, TRecord? data)
    {
        this.Status = status;
        this.Message = message;
        this.Data = data;
    }
    
    public ApiResponse(string status, string message)
    {
        this.Status = status;
        this.Message = message;
    }
    
    
}