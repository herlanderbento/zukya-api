using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Zukya.Domain.Shared.ValueObjects;

namespace Zukya.Infra.Persistence.Converters;

public class ImageToStringConverter() : ValueConverter<Image?, string?>(v => v == null ? null : v.Path,
    v => string.IsNullOrWhiteSpace(v) ? null : new Image(v));
