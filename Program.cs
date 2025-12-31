using Microsoft.Extensions.Configuration;
using OdinProjectAPI.Configuration;
using OdinProjectAPI.GraphQL;
using OdinProjectAPI.WegSubnav;
using System.Text.Json;
using OdinProjectAPI.Inventory;
using OdinProjectAPI.Output;

internal static class Program
{
    public static async Task<int> Main()
    {
        try
        {
            var settings = LoadSettings();
            ValidateSettings(settings);

            using var http = new HttpClient();

            var gqlClient = new GraphQLTransportClient(http, settings.Odin.GraphQLEndPoint!);
            var wegRepo = new WegCardQueryRepository(gqlClient);

            ValidateTiers(settings);
            PrintTierDropdown(settings);

            await RunDiagnosticsAsync(http, gqlClient);
            var cacheRoot = await BuildOrLoadCategoryCacheAsync();

            await RunFilteringDemoAsync(settings, cacheRoot, wegRepo);

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR: " + ex.Message);
            return 1;
        }
    }

    // -------------------------
    // Phase 1: Configuration
    // -------------------------
    private static AppSettings LoadSettings()
    {
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        return config.Get<AppSettings>()
            ?? throw new Exception("Failed to bind configuration.");
    }

    private static void ValidateSettings(AppSettings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.Odin.GraphQLEndPoint))
            throw new Exception("Missing setting: Odin:GraphQLEndPoint");

        if (string.IsNullOrWhiteSpace(settings.OutputFolder))
            throw new Exception("Missing setting: OutputFolder");

        Console.WriteLine("Configuration loaded successfully:");
        Console.WriteLine($"FSAPI Base: {settings.Odin.ForceStructureAPI}");
        Console.WriteLine($"DISAPI Base: {settings.Odin.DISEnumerationAPI}");
        Console.WriteLine($"GraphQL Endpoint: {settings.Odin.GraphQLEndPoint}");
        Console.WriteLine($"Output Folder: {settings.OutputFolder}");
    }

    // -------------------------
    // Phase 2: Quick Diagnostics
    // -------------------------
    private static async Task RunDiagnosticsAsync(HttpClient http, GraphQLTransportClient gqlClient)
    {
        Console.WriteLine("\n--- Diagnostics: GraphQL + image download ---");

        var query = @"
        query WegCards($limit: Int!) {
          wegCardCollection(limit: $limit, offset: 0) {
            name
            images
          }
        }";

        var variables = new { limit = 1 };

        var raw = await gqlClient.ExecuteRawAsync(query, variables);
        var parsed = JsonSerializer.Deserialize<GraphQLResponse<WegCardCollectionData>>(
            raw,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var item = parsed?.Data?.WegCardCollection?.FirstOrDefault();
        var imageService = new WegImageParser();
        var images = imageService.ParseImages(item?.ImagesRaw);

        var firstImageUrl = images.Count > 0
            ? imageService.BuildAbsoluteUrl(images[0].Url)
            : string.Empty;

        Console.WriteLine($"Card: {item?.Name}");
        Console.WriteLine($"First Image Url: {(string.IsNullOrWhiteSpace(firstImageUrl) ? "(no image)" : firstImageUrl)}");

        if (!string.IsNullOrWhiteSpace(firstImageUrl))
        {
            var bytes = await http.GetByteArrayAsync(firstImageUrl);
            Console.WriteLine($"Downloaded {bytes.Length} bytes");
        }
    }

    // -------------------------
    // Phase 3: Categories (subnav)
    // -------------------------
    private static async Task<WegCategoryNode> BuildOrLoadCategoryCacheAsync()
    {
        Console.WriteLine("\n--- Categories: subnav fetch + normalize + load ---");

        await WegSubnavFetcher.WegSubnavAsync();
        await WegSubnavFetcher.InspectParsedAsync();
        await WegSubnavFetcher.BuildAndCacheNormalizedTreeAsync();

        var cache = await WegCategoryRepository.LoadAsync();
        if (cache.RootNodes.Count == 0)
            throw new Exception("weg-categories.json contained zero root nodes.");

        return cache.RootNodes[0];
    }

    // -------------------------
    // Phase 4: Filtering demo
    // -------------------------
    private static async Task RunFilteringDemoAsync(
        AppSettings settings,
        WegCategoryNode root,
        WegCardQueryRepository wegRepo)
    {
        Console.WriteLine("\n--- Filtering demo ---");

        // Domain dropdown
        var domainNode = WegCategoryRepository.FindByVariable(root, "domain")
            ?? throw new Exception("Domain node not found.");

        var domainDropdown = WegCategoryRepository.ToDropdownOptions(domainNode);
        var selectedDomainVariable = domainDropdown.First(o => o.Label == "Air").Value;

        // Weapon System dropdown for selected domain
        var selectedDomainNode = WegCategoryRepository.FindByVariable(root, selectedDomainVariable)
            ?? throw new Exception("Selected domain not found.");

        var weaponSystemDropdown = WegCategoryRepository.ToDropdownOptions(selectedDomainNode);
        //var selectedWeaponSystemVariable = weaponSystemDropdown.First(o => o.Label == "Infantry Weapons").Value;

        var criteria = new WegFilterCriteria
        {
            DomainVariable = selectedDomainVariable,

            WeaponSystemTypeVariable = new List<string>
            {
                "aircraft-42b8bd"
            },

            OriginVariable = new List<string>
            {
                 "china--people-s-republic-of-d6ee02",
                 "russia--rus--f8577e"
            },

            TierKey = new List<string>
            {
                "Tier1",
                "Tier2",
                "Tier3"
            }
        };

        var lucene = LuceneQueryBuilder.Build(criteria, settings.Weg.Tiers);
        Console.WriteLine($"\nLucene Query:\n{lucene}");

        var cards = await wegRepo.GetWegCardsAsync(lucene, limit:5, offset: 0);
        Console.WriteLine($"\nFiltered results returned: {cards.Count}");

        var inventory = new WegSectionsInventoryComparer();

        const int pageSize = 700;
        var offset = 0;
        var total = 0;
        
        while(true)
        {
            var page = await wegRepo.GetWegCardsAsync(lucene, limit: pageSize, offset:offset);
            if (page.Count == 0) break;

            total += page.Count;

            foreach (var card in page)
            {
                var parsedSections = WegSectionsParser.Parse(card.SectionsRaw);
                inventory.AddCardSections(parsedSections);
            }

            Console.WriteLine($"Inventory processed {page.Count} cards (offset {offset}) total {total}");

            if (page.Count < pageSize) break;
            offset += pageSize;
        }

        DumpTop("Top-Level Sections", inventory.SectionCounts, 50);
        DumpTop("Subsections", inventory.SubsectionCounts, 50);
        DumpTop("Properties", inventory.PropertyCounts, 100);


        //var first = cards.FirstOrDefault();
        //Console.WriteLine("\n--- GraphQL payload validation ---");
        //Console.WriteLine($"Sections chars: {first?.SectionsRaw?.Length ?? 0}");
        //Console.WriteLine(first?.SectionsRaw is null ? "No Sections" : first.SectionsRaw);
    }

    // -------------------------
    // Tier helpers
    // -------------------------
    private static void ValidateTiers(AppSettings settings)
    {
        var tiers = settings.Weg.Tiers;
        if (tiers == null || tiers.Count == 0)
            throw new Exception("Tier config missing or empty: Weg:Tiers");

        Console.WriteLine($"\nTier config OK. Loaded {tiers.Count} tiers.");
    }

    private static void PrintTierDropdown(AppSettings settings)
    {
        var tierDropdown = settings.Weg.Tiers
            .Select(t => new DropdownOption { Label = t.Label, Value = t.Key })
            .OrderBy(o => o.Label)
            .ToList();

        Console.WriteLine("\nTier Dropdown:");
        foreach (var tier in tierDropdown)
            Console.WriteLine($"{tier.Label} {tier.Value}");
    }

    private static void DumpTop(string title, Dictionary<string, int> dict, int top)
    {
        Console.WriteLine($"\n=== {title} (top {top}) ===");
        foreach (var kv in dict.OrderByDescending(k => k.Value).Take(top))
            Console.WriteLine($"{kv.Value,4}  {kv.Key}");
    }

}
