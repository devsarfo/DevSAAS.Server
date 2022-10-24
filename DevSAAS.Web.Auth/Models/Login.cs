using System.ComponentModel.DataAnnotations;

namespace DevSAAS.Web.Auth.Models;

public class Login
{
    [Required] public string Username { get; set; } = string.Empty;

    [Required] public string Password { get; set; } = string.Empty;
}