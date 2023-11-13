using System.Text.Json.Serialization;

namespace ConsoleApp4;

[JsonSerializable(typeof(AppSettings))]
[JsonSerializable(typeof(SubstringOrRegexPattern))]
internal partial class MyJsonContext : JsonSerializerContext
{
}