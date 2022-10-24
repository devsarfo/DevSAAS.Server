using DevSAAS.Core.Helpers;

namespace DevSAAS.Core.Identity.Entities;

public class User
{
    private User()
    {
    }

    public User(string name, string email, string phone, string password) : this()
    {
        Name = name;
        Email = email;
        Phone = phone;
        Password = BCrypt.Net.BCrypt.HashPassword(password);
    }

    public string Id { get; } = Guid.NewGuid().ToString();

    public string Photo { get; set; } = FileHelper.GetFilePath("files/default/png");

    public string Name { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public DateTime? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string Email { get; set; } = string.Empty;

    public DateTime? EmailVerifiedAt { get; set; }

    public string Phone { get; set; } = string.Empty;

    public DateTime? PhoneVerifiedAt { get; set; }

    public string Password { get; set; } = string.Empty;

    public string Pin { get; set; } = string.Empty;

    public int Active { get; set; }

    public DateTime CreatedAt { get; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
}