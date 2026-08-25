namespace Zukya.Domain.Shared.ValueObjects;

public class Image : ValueObject
{
    public string Path { get; private set; }

    public Image(string path) => Path = path;

    public override bool Equals(ValueObject? other) => other is Image file && Path == file.Path;

    protected override int GetCustomHashCode() => HashCode.Combine(Path);
}