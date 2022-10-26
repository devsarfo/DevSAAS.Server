namespace DevSAAS.Web.Auth.Responses;

public class AuthResponse
{
    private AuthResponse() { }

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

    public string Id { get; } = string.Empty;

    public string Photo { get; } = string.Empty;

    public string Name { get; } = string.Empty;

    public string Email { get; } = string.Empty;

    public DateTime? EmailVerifiedAt { get; }

    public string Phone { get; } = string.Empty;

    public DateTime? PhoneVerifiedAt { get; }

    public string Token { get; } = string.Empty;
}