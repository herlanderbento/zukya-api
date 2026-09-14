namespace Zukya.Domain.SystemSetting.Entities;

public class SystemSetting
{
    public int Id { get; protected set; }
    public string Key { get; private set; }
    public string Value { get; private set; }
    public string Type { get; private set; }
    public string Description { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public SystemSetting(
        string key,
        string value,
        string type,
        string description
    )
    {
        Key = key;
        Value = value;
        Type = type;
        Description = description;

        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Update(
        string? value,
        string? type,
        string? description
    )
    {
        Value = value ?? Value;
        Type = type ?? Type;
        Description = description ?? Description;

        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
