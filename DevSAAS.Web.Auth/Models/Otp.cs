using Microsoft.Build.Framework;

namespace DevSAAS.Web.Auth.Models;

public class Otp
{
    [Required] 
    public string Phone { get; set; } = string.Empty;

    [Required] 
    public string Code { get; set; } = string.Empty;
}