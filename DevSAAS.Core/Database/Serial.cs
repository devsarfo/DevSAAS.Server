namespace DevSAAS.Core.Database;

public readonly struct SerialInt32
{
    private readonly int _value;

    public SerialInt32(int value)
    {
        this._value = value;
    }

    public override string ToString()
    {
        return _value.ToString();
    }

    public static implicit operator SerialInt32(int value)
    {
        return new(value);
    }

    public static implicit operator int(SerialInt32 value)
    {
        return value._value;
    }

    public static object ToObject(SerialInt32 value)
    {
        return value._value;
    }
}

public readonly struct SerialInt64
{
    private readonly long _value;

    public SerialInt64(long value)
    {
        this._value = value;
    }

    public override string ToString()
    {
        return _value.ToString();
    }

    public static implicit operator SerialInt64(long value)
    {
        return new(value);
    }

    public static implicit operator long(SerialInt64 value)
    {
        return value._value;
    }
}