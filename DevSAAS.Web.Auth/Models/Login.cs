
using System.ComponentModel.DataAnnotations;

namespace DevSAAS.Web.Auth.Models;

public class Login
{
    [Required]
    public string Username { get; set; }
    
    [Required]
    public string Password { get; set; }
}