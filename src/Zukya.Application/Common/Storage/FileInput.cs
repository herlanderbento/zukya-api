namespace Zukya.Application.Common.Storage;

public record FileInput(string Extension, Stream FileStream, string ContentType);
