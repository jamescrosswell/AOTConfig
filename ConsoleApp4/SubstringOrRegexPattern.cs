using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace ConsoleApp4;

/// <summary>
/// Provides a pattern that can be used to match against other strings as either a substring or regular expression.
/// </summary>
[TypeConverter(typeof(SubstringOrRegexPatternTypeConverter))]
public class SubstringOrRegexPattern
{
    private readonly Regex? _regex;
    private readonly string? _substring;
    private readonly StringComparison _stringComparison;

    /// <summary>
    /// Constructs a <see cref="SubstringOrRegexPattern"/> instance.
    /// </summary>
    /// <param name="substringOrRegexPattern">The substring or regular expression pattern to match on.</param>
    /// <param name="comparison">The string comparison type to use when matching.</param>
    public SubstringOrRegexPattern(
        string substringOrRegexPattern,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        _substring = substringOrRegexPattern;
        _stringComparison = comparison;
        _regex = TryParseRegex(substringOrRegexPattern, comparison);
    }

    /// <summary>
    /// Constructs a <see cref="SubstringOrRegexPattern"/> instance.
    /// </summary>
    /// <param name="regex"></param>
    /// <remarks>
    /// Use this constructor when you need to control the regular expression matching options.
    /// We recommend setting at least <see cref="RegexOptions.Compiled"/> for performance, and
    /// <see cref="RegexOptions.CultureInvariant"/> (unless you have culture-specific matching needs).
    /// The <see cref="SubstringOrRegexPattern(string, StringComparison)"/> constructor sets these by default.
    /// </remarks>
    public SubstringOrRegexPattern(Regex regex) => _regex = regex;

    /// <summary>
    /// Implicitly converts a <see cref="string"/> to a <see cref="SubstringOrRegexPattern"/>.
    /// </summary>
    /// <param name="substringOrRegexPattern"></param>
    public static implicit operator SubstringOrRegexPattern(string substringOrRegexPattern)
    {
        return new SubstringOrRegexPattern(substringOrRegexPattern);
    }

    /// <inheritdoc />
    public override string ToString() => _substring ?? _regex?.ToString() ?? "";

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return
            (obj is SubstringOrRegexPattern pattern)
            && pattern.ToString() == ToString();
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }

    internal bool IsMatch(string str) =>
        _substring == ".*" || // perf shortcut
        (_substring != null && str.Contains(_substring, _stringComparison)) ||
        _regex?.IsMatch(str) == true;

    private static Regex? TryParseRegex(string pattern, StringComparison comparison)
    {
        try
        {
            var regexOptions = RegexOptions.Compiled;

            if (comparison is
                StringComparison.InvariantCulture or
                StringComparison.InvariantCultureIgnoreCase or
                StringComparison.Ordinal or
                StringComparison.OrdinalIgnoreCase)
            {
                regexOptions |= RegexOptions.CultureInvariant;
            }

            if (comparison is
                StringComparison.CurrentCultureIgnoreCase or
                StringComparison.InvariantCultureIgnoreCase or
                StringComparison.OrdinalIgnoreCase)
            {
                regexOptions |= RegexOptions.IgnoreCase;
            }

            return new Regex(pattern, regexOptions);
        }
        catch
        {
            // not a valid regex
            return null;
        }
    }
}

public class SubstringOrRegexPatternConverter : JsonConverter<SubstringOrRegexPattern>
{
    public override SubstringOrRegexPattern Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString()!;
    }

    public override void Write(Utf8JsonWriter writer, SubstringOrRegexPattern value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}

internal class SubstringOrRegexPatternTypeConverter : TypeConverter
{
    // This class allows the TracePropagationTargets option to be set from config, such as appSettings.json

    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) =>
        sourceType == typeof(string);

    public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value) =>
        new SubstringOrRegexPattern((string)value);
}