namespace DevSAAS.Core.Identity.Entities;

public class Otp
{
    private Otp() {}

    public Otp(string userId, string code) : this()
    {
        UserId = userId;
        Code = code;
    }

    public string Id { get; } = Guid.NewGuid().ToString();
    
    public string UserId { get; init;  }
    
    public string Code { get; init;  }

    public int Active { get; init; } = 1;
    
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; init;  }
    
    public DateTime? DeletedAt { get; init;  }
}