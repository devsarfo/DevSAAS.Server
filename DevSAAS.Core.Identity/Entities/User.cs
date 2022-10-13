namespace DevSAAS.Core.Identity.Entities;

public class User
{
    protected User()
    {
        this.Id = Guid.NewGuid().ToString();
        this.CreatedAt = DateTime.UtcNow;
    }

    public User(string photo, string firstName, string lastName, string dob, string gender, string email, string phone, string password, int active)
    {
        Photo = photo;
        FirstName = firstName;
        LastName = lastName;
        Dob = dob;
        Gender = gender;
        Email = email;
        Phone = phone;
        Password = password;
        Active = active;
    }

    public String Id { get; }
    
    public string Photo { get; }
    
    public string Name { get; }
    
    public string FirstName { get; }
    
    public string LastName { get; }
    
    public string Dob { get; }
    
    public string Gender { get; }
    
    public string Email { get; }
    
    public DateTime EmailVerifiedAt { get; }
    
    public string Phone { get; }
    
    public DateTime PhoneVerifiedAt { get; }
    
    public string Password { get; }
    
    public string Pin { get; }
    
    public int Active { get; }
    public DateTime CreatedAt { get; }
    
    public DateTime UpdatedAt { get; }
    
    public DateTime DeletedAt { get; }
}