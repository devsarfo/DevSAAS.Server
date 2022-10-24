using DevSAAS.Core.Identity.Entities;

namespace DevSAAS.Web.Auth.Responses;

public class ProfileResponse
{
    public ProfileResponse()
    {
    }

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


    public string Id { get; }

    public string Photo { get; }

    public string Name { get; }

    public string FirstName { get; }

    public string LastName { get; }

    public DateTime? DateOfBirth { get; }

    public string? Gender { get; }

    public string Email { get; }

    public DateTime? EmailVerifiedAt { get; }

    public string Phone { get; }

    public DateTime? PhoneVerifiedAt { get; }

    public int Active { get; }

    public DateTime CreatedAt { get; }

    public DateTime? UpdatedAt { get; }
}