/* PURPOSE:
   This file serves as the application entry point and orchestration layer.
   Program.cs does not contain business logic or data models.
*/


//Load appsettings.json, bind it to strongly-typed C# classes,validate required values, and print them to the console.
//The orchestration/wiring of the program

using Microsoft.Extensions.Configuration;
using OdinProjectAPI.Configuration;
using OdinProjectAPI.GraphQL;
using OdinProjectAPI.WegSubnav;
using System.Collections.Generic;
using System.Text.Json;

try
    //PHASE 1: LOAD AND VALIDATE CONFIGURATION
    /*
        ConfigurationBuilder creates the configuration pipeline.
        1. Where to look (BaseDirectory = runtime folder)
        2. What sources to load (JSON file)
        3. To build a final IConfiguration object
    */
{
    //It builds a key–value tree in memory. So maps the strings in appsettings to their values
    //Docs Configuration: https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration#basic-example
    //Docs IConfiguration: https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.iconfiguration?view=net-10.0-pp
    IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile(
            path: "appsettings.json",
            optional: false,
            reloadOnChange: true
        )
        //Docs Build(): https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.configurationbuilder.build?view=net-10.0-pp#microsoft-extensions-configuration-configurationbuilder-build
        .Build();

    /*
    Reads the JSON file
    Creates an AppSettings object
    Fills in properties by name
    */
    //Docs .Get: https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.configurationbinder.get?view=net-9.0-pp
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

    var client = new GraphQLTransportClient(http, settings.Odin.GraphQLEndPoint);

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

    var variables = new
    {
        limit = 10
    };

    // - ExecuteRawAsync sends the query to the server
    // - await pauses execution until the HTTP response is received
    // - result will contain the raw JSON response as a string
    var result = await client.ExecuteRawAsync(query, variables);

    Console.WriteLine("GraphQL Response");
    Console.WriteLine(result);

    //PHASE 3: Send Test Query

    // Convert the raw JSON string into a typed object:
    // GraphQLResponse<TypenameData> represents: { "data": { "__typename": "Query" } }
    var wegParsed = JsonSerializer.Deserialize<GraphQLResponse<WegCardCollectionData>>(result);

    var imageService = new WegImageParser();

    var item = wegParsed?.Data?.WegCardCollection?.FirstOrDefault();

    var images = imageService.ParseImages(item?.ImagesRaw);

    var firstImageUrl = images.Count > 0 ? imageService.BuildAbsoluteUrl(images[0].Url) : "(no image)";

    var bytes = await http.GetByteArrayAsync(firstImageUrl);

    Console.WriteLine($"Card: {item?.Name}");
    Console.WriteLine($"Url: {item?.ImagesRaw}");
    Console.WriteLine($"First Image Url: {firstImageUrl}");
    Console.WriteLine($"Downloaded {bytes.Length} bytes");

    await WegSubnavFetcher.WegSubnavAsync();
    await WegSubnavFetcher.InspectParsedAsync();
    await WegSubnavFetcher.BuildAndCacheNormalizedTreeAsync();

    //Testing dropdowns
    var cache = await WegCategoryRepository.LoadAsync();

    var root = cache.RootNodes[0];

    var domainNode = WegCategoryRepository.FindByVariable(root, "domain")
        ?? throw new Exception("Domain node not found.");

    var domainDropdown  = WegCategoryRepository.ToDropdownOptions(domainNode);

    Console.WriteLine("Domain Dropdown:");
    foreach (var option in domainDropdown)
    {
        Console.WriteLine($"{option.Label} {option.Value}");
    }

    var selectedDomainVariable = domainDropdown.First(o => o.Label == "Land").Value;

    var selectedDomainNode = WegCategoryRepository.FindByVariable(root, selectedDomainVariable) ?? throw new InvalidOperationException("Selected domain not found.");

    var weaponSystemDropdown = WegCategoryRepository.ToDropdownOptions(selectedDomainNode);

    Console.WriteLine("\nWeapon System Types (Land):");
    foreach (var option in weaponSystemDropdown)
    {
        Console.WriteLine($"{option.Label} {option.Value}");
    }

    var criteria = new WegFilterCriteria
    {
        DomainVariable = selectedDomainVariable,
        WeaponSystemTypeVariable = weaponSystemDropdown.First().Value
    };

    Console.WriteLine("\nCriteria:");
    Console.WriteLine($"Domain: {criteria.DomainVariable}");
    Console.WriteLine($"Weapon System: {criteria.WeaponSystemTypeVariable}");

    var lucene = LuceneQueryBuilder.Build(criteria);
    Console.WriteLine($"\nLucene Query:\n{lucene}");

    var wegCardService = new WegCardQueryRepository(client);

    var cards = await wegCardService.GetWegCardsAsync(lucene, limit: 5);

    Console.WriteLine($"\nFiltered results returned: {cards.Count}");

    foreach (var card in cards)
    {
        Console.WriteLine($"Name: {card.Name}");

        if (card.Origin != null && card.Origin.Count > 0)
        {
            Console.WriteLine($"Origin: {string.Join(", ", card.Origin.Select(o => o.Name))}");
        }
    }

    static void ValidateTiers(AppSettings settings)
    {
        var tiers = settings.Weg.Tiers;

        Console.WriteLine($"Tier config OK. Loaded {tiers.Count} tiers.");
    }

    ValidateTiers(settings);

    var tierDropdown = settings.Weg.Tiers.Select(t => new DropdownOption {  Label = t.Label, Value = t.Key}).OrderBy(o => o.Label).ToList();

    foreach (var tier in tierDropdown)
    {
        Console.WriteLine($"{tier.Label} {tier.Value}");
    }

}
catch (Exception ex)
{
    Console.WriteLine("ERROR:" + ex.Message);
    //The 1 exit code indicates unknown errors and acts as a catch all. 
    Environment.ExitCode = 1;
}

