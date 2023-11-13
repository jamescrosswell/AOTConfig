namespace ConsoleApp4;

public class AppSettings
{
    public string? ExampleString { get; set; }
    public int? Number { get; set; }
    
    // Uncomment the following to break source generation...   
    public SubstringOrRegexPattern? Pattern { get; set; }
    
    // Uncomment the following to break source generation...   
    public Func<double?>? SampleRate { get; set; }
}