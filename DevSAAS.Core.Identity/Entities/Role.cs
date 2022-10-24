namespace DevSAAS.Core.Identity.Entities;

public class Role
{
    public Role()
    {
    }

    public Role(string name, string code, int isDefault, int active)
    {
        Name = name;
        Code = code;
        IsDefault = isDefault;
        Active = active;
    }

    public string Id { get; } = Guid.NewGuid().ToString();

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public int IsDefault { get; set; }

    public int Active { get; set; }

    public DateTime CreatedAt { get; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
}