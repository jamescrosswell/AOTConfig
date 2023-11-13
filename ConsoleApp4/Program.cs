using ConsoleApp4;
using Microsoft.Extensions.Configuration;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("AppSettings.json");

var appSettings = new AppSettings();

var config = builder.Build();

var configSetup = new AppSettingsSetup(config);
configSetup.Configure(appSettings);

Console.WriteLine(appSettings?.ExampleString);
Console.WriteLine(appSettings?.Number);
Console.WriteLine(appSettings?.Pattern);