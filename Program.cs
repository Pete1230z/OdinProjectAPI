/* PURPOSE:
   This file serves as the application entry point and orchestration layer.
   Program.cs does not contain business logic or data models.
*/


//Load appsettings.json, bind it to strongly-typed C# classes,validate required values, and print them to the console.
//The orchestration/wiring of the program

using Microsoft.Extensions.Configuration;
using OdinProjectAPI.Configuration;
using OdinProjectAPI.Services;
using System.Text.Json;
using OdinProjectAPI.DTOs;

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
    Reads the JSON file
    Creates an AppSettings object
    Fills in properties by name
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

    // HttpClient is responsible for sending HTTP requests (POST, GET, etc.).
    // This object manages network connections and should be reused rather than recreated repeatedly.
    //Documentation https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-10.0
    using var http = new HttpClient();

    var client = new GraphQLClient(http, settings.Odin.GraphQLEndPoint);

    // This is written in the GraphQL query language, not C#.
    // "__typename" is a special built-in GraphQL field that tells us
    // the name of the root type returned by the server.
    var query = @"
    query {
      wegCardCollection(limit: 1, offset: 0) {
        name
        images
      }
    }";


    // - ExecuteRawAsync sends the query to the server
    // - await pauses execution until the HTTP response is received
    // - result will contain the raw JSON response as a string
    var result = await client.ExecuteRawAsync(query);

    Console.WriteLine("GraphQL Response");
    Console.WriteLine(result);

    //PHASE 3: Send Test Query

    // Convert the raw JSON string into a typed object:
    // GraphQLResponse<TypenameData> represents: { "data": { "__typename": "Query" } }
    var wegParsed = JsonSerializer.Deserialize<GraphQLResponse<WegCardCollectionData>>(result);

    var imageService = new WebImageService();

    var item = wegParsed?.Data?.WegCardCollection?.FirstOrDefault();

    var images = imageService.ParseImages(item?.ImagesRaw);

    var firstImageUrl = images.Count > 0 ? imageService.BuildAbsoluteUrl(images[0].Url) : "(no image)";

    var bytes = await http.GetByteArrayAsync(firstImageUrl);

    Console.WriteLine($"Card: {item?.Name}");
    Console.WriteLine($"Url: {item?.ImagesRaw}");
    Console.WriteLine($"First Image Url: {firstImageUrl}");
    Console.WriteLine($"Downloaded {bytes.Length} bytes");

}
catch (Exception ex)
{
    Console.WriteLine("ERROR:" + ex.Message);
    //The 1 exit code indicates unknown errors and acts as a catch all. 
    Environment.ExitCode = 1;
}

