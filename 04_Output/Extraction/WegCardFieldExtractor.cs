using OdinProjectAPI.GraphQL;
using OdinProjectAPI.Inventory;
using System.Data;
using System.Security.Principal;

namespace OdinProjectAPI.Output;

public static class WegCardFieldExtractor
{
    // Extracts values for one card using a field specification
    // because we want one generic extractor for all systems (aircraft, infantry, tanks, etc.)
    public static Dictionary<string, string> Extract(WegCardItem card, IReadOnlyDictionary<string, string[]> fieldSpec)
    {
        // Holds the final column -> value output for this card
        // case-insensitive because column names should not break due to casing differences
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // Parse raw JSON sections into structured objects
        // because ODIN stores sections as JSON strings
        var sections = WegSectionsParser.Parse(card.SectionsRaw);

        // Build a lookup like "System::Maximum Range" -> "500 km"
        // because scanning lists repeatedly would be slow and messy
        var index = WegSectionIndexBuilder.BuildIndex(sections);

        // Loop over each column defined in the spec
        foreach (var kvp in fieldSpec)
        {
            var columnName = kvp.Key;
            var fallbackKeys = kvp.Value;

            // Find the first matching value using the fallback order
            result[columnName] = FirstMatch(index, fallbackKeys);
        }

        return result;
    }

    // Finds the first non-empty value in the index
    // because ODIN uses many different property names for the same concept
    private static string FirstMatch(Dictionary<string, string> index, IEnumerable<string> fallbacks)
    {
        foreach (var key in fallbacks)
        {
            // Try to find a value like "System::Maximum Range"
            if (index.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value))
            {
                return value.Trim();
            }
        }

        // Return empty string instead of null
        // because Excel output should be clean and predictable
        return string.Empty;
    }
}