using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Options;

namespace ConsoleApp4;

internal class AppSettingsSetup : IConfigureOptions<AppSettings>
{
    private readonly string? rawConfig;

    public AppSettingsSetup(IConfigurationRoot config)
    {
        ArgumentNullException.ThrowIfNull(config);
        foreach (var provider in config.Providers)
        {
            if (provider is JsonConfigurationProvider { Source.Path: not null } jsonProvider)
            {
                rawConfig = GetRawConfig(jsonProvider.Source.Path, "Settings");
            }
        }
    }

    public virtual void Configure(AppSettings options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }
        var settings = ReadSettings();
        if (settings == null)
        {
            return;
        }
        // Apply any non-null settings - note the object we read the settings into wouldn't necessarily have to be
        // an `AppSettings` class... it could be a `PersistedAppSettings` class, for example, where everything was
        // nullable, to make it easier to work out what to overwrite/bind and what to leave alone. 
        options.ExampleString = settings.ExampleString ?? options.ExampleString;
        options.Number = settings.Number ?? options.Number;
        options.Pattern = settings.Pattern ?? options.Pattern;
    }

    private AppSettings? ReadSettings()
    {
        var jsonContext = new MyJsonContext(
            new JsonSerializerOptions()
            {
                Converters = { new SubstringOrRegexPatternConverter() }
            });
        var settings = JsonSerializer.Deserialize(rawConfig!, typeof(AppSettings), jsonContext);
        return (AppSettings?)settings;
    }

    public static string? GetRawConfig(string filePath, string sectionName)
    {
        try
        {
            var jsonString = File.ReadAllText(filePath);
            using JsonDocument doc = JsonDocument.Parse(jsonString);
            JsonElement root = doc.RootElement;

            if (root.TryGetProperty(sectionName, out JsonElement section))
            {
                return section.GetRawText();
            }

            return null; 
        }
        catch (Exception ex)
        {
            // Handle or log the exception as necessary
            Console.WriteLine($"Error reading config file: {ex.Message}");
            return null;
        }
    }    
}