using DevSAAS.Core.Identity.Entities;

namespace DevSAAS.Web.Auth.Responses;

public class ProfileResponse
{
    public ProfileResponse() { }

    public ProfileResponse(User user)
    {
        Id = user.Id;
        Photo = user.Photo;
        Name = user.Name;
        FirstName = user.FirstName;
        LastName = user.LastName;
        DateOfBirth = user.DateOfBirth;
        Gender = user.Gender;
        Email = user.Email;
        EmailVerifiedAt = user.EmailVerifiedAt;
        Phone = user.Phone;
        PhoneVerifiedAt = user.PhoneVerifiedAt;
        Active = user.Active;
        CreatedAt = user.CreatedAt;
        UpdatedAt = user.UpdatedAt;
    }


    public string Id { get; } = string.Empty;

    public string Photo { get; } = string.Empty;

    public string Name { get; } = string.Empty;

    public string FirstName { get; } = string.Empty;

    public string LastName { get; } = string.Empty;

    public DateTime? DateOfBirth { get; }

    public string? Gender { get; } = string.Empty;

    public string Email { get; } = string.Empty;

    public DateTime? EmailVerifiedAt { get; }

    public string Phone { get; } = string.Empty;

    public DateTime? PhoneVerifiedAt { get; }

    public int Active { get; }

    public DateTime CreatedAt { get; }

    public DateTime? UpdatedAt { get; }
}