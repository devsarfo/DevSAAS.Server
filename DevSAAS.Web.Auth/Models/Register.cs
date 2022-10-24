using System.ComponentModel.DataAnnotations;

namespace DevSAAS.Web.Auth.Models;

public class Register
{
    [Required] public string Name { get; set; } = string.Empty;

    [Required] public string Email { get; set; } = string.Empty;

    [Required] public string Phone { get; set; } = string.Empty;

    [Required] public string Password { get; set; } = string.Empty;
}