namespace Movu.Infra.Storage;

public class StorageServiceOptions
{
    public const string ConfigurationSection = "Storage";
    public string BucketName { get; set; } = null!;
    public string Region { get; set; } = null!;
    public string AccessKey { get; set; } = null!;
    public string SecretKey { get; set; } = null!;
    public string? EndpointUrl { get; set; }
    public string? PublicUrl { get; set; }
}
