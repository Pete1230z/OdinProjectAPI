//Load appsettings.json, bind it to strongly-typed C# classes,validate required values, and print them to the console.

using Microsoft.Extensions.Configuration;


try
    /*
        ConfigurationBuilder creates the configuration pipeline.
        1. Where to look (BaseDirectory = runtime folder)
        2. What sources to load (JSON file)
        3. To build a final IConfiguration object
    */
{
    var baseDir = AppContext.BaseDirectory;
    var settingsPath = Path.Combine(baseDir, "appsettings.json");

    Console.WriteLine("Basedirectory: " + baseDir);
    Console.WriteLine("Appsettings Path: " + settingsPath);
    Console.WriteLine("Exists?" + File.Exists(settingsPath));

    IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile(
            path: "appsettings.json",
            optional: false,
            reloadOnChange: true
        )
        .Build();

    /*
    Bind configuration values into strongly-typed C# objects.
    This uses Microsoft.Extensions.Configuration.Binder.
    */
    var settings = config.Get<AppSettings>()
        ?? throw new Exception("Failed to bind configuration.");

    //If API returns nothing fail
    if (string.IsNullOrWhiteSpace(settings.Odin.GraphQLEndPoint))
        throw new Exception("Missing setting: Odin:GraphQLEndpoint");

    if (string.IsNullOrWhiteSpace(settings.OutputFolder))
        throw new Exception("Missing setting: OutputFolder");

    /*
    Output values to confirm everything loaded correctly.
    This is just a verification step.
    */
    Console.WriteLine("Configuration loaded successfully:");
    Console.WriteLine($"FSAPI Base: {settings.Odin.ForceStructureAPI}");
    Console.WriteLine($"DISAPI Base: {settings.Odin.DISEnumerationAPI}");
    Console.WriteLine($"GraphQL Endpoint: {settings.Odin.GraphQLEndPoint}");
    Console.WriteLine($"Output Folder: {settings.OutputFolder}");
}
catch (Exception ex)
{
    Console.WriteLine("ERROR:" + ex.Message);
    //The 1 exit code indicates unknown errors and acts as a catch all. 
    Environment.ExitCode = 1;
}

public sealed class OdinSettings
{
    //get and set accessors perform no other operation than setting or retrieving a value
    public string? ForceStructureAPI {  get; set; }
    public string? DISEnumerationAPI { get; set; }
    public string? GraphQLEndPoint { get; set; }

}

public sealed class AppSettings
{
    //References OdinSettings instead of a general other type such as a string
    public OdinSettings Odin { get; set; } = new();
    public string? OutputFolder { get; set; }
}

