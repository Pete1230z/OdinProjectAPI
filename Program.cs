using Microsoft.Extensions.Configuration;
using OdinProjectAPI.Configuration;
using OdinProjectAPI.GraphQL;
using OdinProjectAPI.WegSubnav;
using System.Text.Json;

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
        var selectedDomainVariable = domainDropdown.First(o => o.Label == "Land").Value;

        // Weapon System dropdown for selected domain
        var selectedDomainNode = WegCategoryRepository.FindByVariable(root, selectedDomainVariable)
            ?? throw new Exception("Selected domain not found.");

        var weaponSystemDropdown = WegCategoryRepository.ToDropdownOptions(selectedDomainNode);
        var selectedWeaponSystemVariable = weaponSystemDropdown.First().Value;

        var criteria = new WegFilterCriteria
        {
            DomainVariable = selectedDomainVariable,
            WeaponSystemTypeVariable = selectedWeaponSystemVariable,
            TierKey = "Tier3"
        };

        var lucene = LuceneQueryBuilder.Build(criteria, settings.Weg.Tiers);
        Console.WriteLine($"\nLucene Query:\n{lucene}");

        var cards = await wegRepo.GetWegCardsAsync(lucene, limit: 5);
        Console.WriteLine($"\nFiltered results returned: {cards.Count}");

        // Build origin dropdown from results
        var originOptions = cards
            .Where(c => c.Origin != null)
            .SelectMany(c => c.Origin!)
            .Where(o => !string.IsNullOrWhiteSpace(o?.VelocityVar) && !string.IsNullOrWhiteSpace(o?.Name))
            .GroupBy(o => o!.VelocityVar!, StringComparer.OrdinalIgnoreCase)
            .Select(g => new DropdownOption { Value = g.Key, Label = g.First()!.Name! })
            .OrderBy(o => o.Label)
            .ToList();

        Console.WriteLine("\nOrigin Dropdown:");
        foreach (var opt in originOptions)
            Console.WriteLine($"Value: {opt.Value} Label: {opt.Label}");

        if (originOptions.Count == 0)
            throw new Exception("No origin options detected.");

        // Apply origin filter
        var selectedOrigin = originOptions.First();
        criteria.OriginVariable = selectedOrigin.Value;

        var luceneWithOrigin = LuceneQueryBuilder.Build(criteria, settings.Weg.Tiers);
        Console.WriteLine($"\nLucene Query with Origin:\n{luceneWithOrigin}");

        var cardsWithOrigin = await wegRepo.GetWegCardsAsync(luceneWithOrigin, limit: 5);

        foreach (var card in cardsWithOrigin)
        {
            Console.WriteLine($"\nName: {card.Name}");
            var origins = card.Origin ?? new List<DotCategoryDTO>();

            foreach (var o in origins)
                Console.WriteLine($"Origin: {o.Name} | var={o.VelocityVar}");

            var matches = origins.Any(o =>
                string.Equals(o.VelocityVar, selectedOrigin.Value, StringComparison.OrdinalIgnoreCase));

            Console.WriteLine($"Matches selected origin? {matches}");
        }
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
}
