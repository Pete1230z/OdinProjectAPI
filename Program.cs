//Load appsettings.json, bind it to strongly-typed C# classes,validate required values, and print them to the console.
//The orchestration/wiring of the program

using Microsoft.Extensions.Configuration;
using OdinProjectAPI.Configuration;
using OdinProjectAPI.Services;

try
    //PHASE 1: LOAD AND VALIDATE CONFIGURATION
    /*
        ConfigurationBuilder creates the configuration pipeline.
        1. Where to look (BaseDirectory = runtime folder)
        2. What sources to load (JSON file)
        3. To build a final IConfiguration object
    */
{
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

    //PHASE 2: USE THE GRAPHQL CLIENT

    using var http = new HttpClient();

    var client = new GraphQLClient(http, settings.Odin.GraphQLEndPoint);

    var query = @"query {__typename}";

    var result = await client.ExecuteRawAsync(query);

    Console.WriteLine("GraphQL Response");
    Console.WriteLine(result);
}
catch (Exception ex)
{
    Console.WriteLine("ERROR:" + ex.Message);
    //The 1 exit code indicates unknown errors and acts as a catch all. 
    Environment.ExitCode = 1;
}

