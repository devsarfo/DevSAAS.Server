using DevSAAS.Core.Helpers;

namespace DevSAAS.Core.Identity.Entities;

public class User
{
    private User() {}

    public User(string name, string email, string phone, string password) : this()
    {
        Name = name;
        Email = email;
        Phone = phone;
        Password = BCrypt.Net.BCrypt.HashPassword(password);
    }

    public string Id { get; } = Guid.NewGuid().ToString();

    public string Photo { get; init; } = FileHelper.GetFilePath("files/default/png");
    
    public string Name { get; init;  }
    
    public string FirstName { get; init;  }
    
    public string LastName { get; init;  }
    
    public DateTime? Dob { get; init;  }
    
    public string? Gender { get; init;  }
    
    public string Email { get; init;  }
    
    public DateTime? EmailVerifiedAt { get; init;  }
    
    public string Phone { get; init;  }
    
    public DateTime? PhoneVerifiedAt { get; init;  }
    
    public string Password { get; init;  }
    
    public string Pin { get; init;  }
    
    public int Active { get; init;  }
    
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; init;  }
    
    public DateTime? DeletedAt { get; init;  }
}