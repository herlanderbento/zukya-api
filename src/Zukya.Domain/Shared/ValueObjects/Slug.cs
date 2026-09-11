using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Zukya.Domain.Shared.ValueObjects;

public class Slug : ValueObject
{
    public string Value { get; }

    private Slug(string value)
    {
        Value = value;
    }

    public static Slug Create(string value) => new(value);

    public static Slug CreateFromText(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("Text cannot be empty.", nameof(text));

        var slugText = RemoveDiacritics(text)
            .ToLower()
            .Trim()
            .Replace(" ", "-")
            .Replace("_", "-");

        slugText = Regex.Replace(slugText, @"[^\w-]", "");
        slugText = Regex.Replace(slugText, @"-{2,}", "-");
        slugText = slugText.Trim('-');

        var code = GenerateHexadecimalCode();

        var slug = $"{slugText}-{code}";

        return new Slug(slug);
    }

    private static string RemoveDiacritics(string text)
    {
        return string.Concat(
            text.Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
        ).Normalize(NormalizationForm.FormC);
    }

    private static string GenerateHexadecimalCode()
    {
        using (var rng = RandomNumberGenerator.Create())
        {
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            var number = BitConverter.ToInt32(bytes, 0) & 0x7FFFFFFF;
            return number.ToString("x");
        }
    }


    public override bool Equals(ValueObject? other) => other is Slug slug && slug.Value == Value;

    protected override int GetCustomHashCode()
        => HashCode.Combine(Value);
}
