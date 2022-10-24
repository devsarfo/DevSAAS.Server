using System.ComponentModel.DataAnnotations;

namespace DevSAAS.Web.Auth.Models;

public class ResendOtp
{
    [Required] public string Phone { get; set; } = string.Empty;
}