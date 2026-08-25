namespace Zukya.Application.Common.Storage;

public static class StorageFileName
{
    public static string Create(Guid id, string propertyName, string extension)
    {
        var cleanExtension = extension.Replace(".", "");
        var uniqueId = Guid.NewGuid();
        return $"{id}/{propertyName.ToLower()}/{uniqueId}.{cleanExtension}";
    }
}
