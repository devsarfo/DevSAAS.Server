using System.ComponentModel.DataAnnotations;

namespace DevSAAS.Web.Auth.Models;

public class Register
{
    [Required]
    public string Name { get; set; }
    
    [Required]
    public string Email { get; set; }
    
    [Required]
    public string Phone { get; set; }
    
    [Required]
    public string Password { get; set; }
}