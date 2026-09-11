using System.Globalization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Zukya.Infra.Persistence.Converters;

public class SnakeCaseEnumToStringConverter<TEnum>() : ValueConverter<TEnum, string>(v => ToSnakeCase(v.ToString()),
    v => Enum.Parse<TEnum>(ToPascalCase(v)))
    where TEnum : struct, Enum
{
    private static string ToSnakeCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        var result = new List<char>();
        for (var i = 0; i < value.Length; i++)
        {
            var c = value[i];
            if (char.IsUpper(c) && i > 0) result.Add('_');
            result.Add(char.ToLower(c, CultureInfo.InvariantCulture));
        }

        return new string(result.ToArray());
    }

    private static string ToPascalCase(string value)
    {
        return string.Concat(
            value
                .Split('_', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => char.ToUpper(s[0]) + s.Substring(1))
        );
    }
}
