namespace DevSAAS.Core.Configuration.Entities;

public class Setting
{
    
    private Setting() {}

    public Setting(string type, string element, string label, string key, string value, string options = null) : this()
    {
        Type = type;
        Element = element;
        Label = label;
        Key = key;
        Value = value;
        Options = options;
    }

    public string Id { get; } = Guid.NewGuid().ToString();
    
    public string Type { get; init;  }
    
    public string Element { get; init;  }
    
    public string Label { get; init;  }
    
    public string Key { get; init;  }
    
    public string Value { get; init;  }
    
    public string? Options { get; init;  }
    
    public int Active { get; init;  }
    
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; init;  }
    
    public DateTime? DeletedAt { get; init;  }
}