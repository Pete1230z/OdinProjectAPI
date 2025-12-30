using System.Text.Json;

namespace OdinProjectAPI.Inventory;

public static class WegSectionsParser
{
    public static List<WegSection> Parse(string? sectionsRaw)
    {
        //If ODIN returns null/empty treat as no sections
        if (string.IsNullOrWhiteSpace(sectionsRaw))
            return new List<WegSection>();

        //Parse the JSON string into List<WegSection?
        try
        {
            //Takes the raw JSON and deserialize into typed objects
            return JsonSerializer.Deserialize<List<WegSection>>(sectionsRaw, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<WegSection>();
        } catch (JsonException)
        {
            return new List<WegSection>();
        }
    }
}