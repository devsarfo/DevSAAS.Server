namespace DevSAAS.Core.Identity.Entities;

public class Otp
{
    private Otp()
    {
    }

    public Otp(string userId, string code) : this()
    {
        UserId = userId;
        Code = code;
    }

    public string Id { get; } = Guid.NewGuid().ToString();

    public string UserId { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public int Active { get; set; } = 1;

    public DateTime CreatedAt { get; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
}