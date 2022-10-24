namespace DevSAAS.Web.Auth.Responses;

public class AuthResponse
{
    private AuthResponse()
    {
    }

    public AuthResponse(string id, string photo, string name, string email, DateTime? emailVerifiedAt, string phone,
        DateTime? phoneVerifiedAt, string token)
    {
        Id = id;
        Photo = photo;
        Name = name;
        Email = email;
        EmailVerifiedAt = emailVerifiedAt;
        Phone = phone;
        PhoneVerifiedAt = phoneVerifiedAt;
        Token = token;
    }

    public string Id { get; }

    public string Photo { get; }

    public string Name { get; }

    public string Email { get; }

    public DateTime? EmailVerifiedAt { get; }

    public string Phone { get; }

    public DateTime? PhoneVerifiedAt { get; }

    public string Token { get; }
}