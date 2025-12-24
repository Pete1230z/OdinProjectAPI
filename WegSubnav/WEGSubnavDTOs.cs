/*
PURPOSE:
Defines raw Data Transfer Objects (DTOs) that mirror the exact JSON structure
returned by the DotCMS WEG subnavigation endpoint.

These models exist only to support deserialization and structural inspection.
They intentionally preserve the original API shape (including dictionary-based
children) and contain no business logic or normalization.

These DTOs should NOT be used directly by the UI or filtering logic.
*/


using System.Text.Json.Serialization;

namespace OdinProjectAPI.WegSubnav;

// Matches: { "content": { ... } }
public sealed class WegSubnavResponse
{
    // Maps JSON key "data" -> C# property Data
    [JsonPropertyName("content")]

    public WegSubnavNodeRaw? Content { get; set; }
}

// Matches nodes like:
// { "name": "...", "key": "...", "variable": "...", "inode": "...", "children": { ... } }
public sealed class WegSubnavNodeRaw
{
    // Maps JSON key "data" -> C# property Data
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    // Maps JSON key "data" -> C# property Data
    [JsonPropertyName("key")]
    public string? Key { get; set; }

    // Maps JSON key "data" -> C# property Data
    [JsonPropertyName("variable")]
    public string? Variable { get; set; }

    // Maps JSON key "data" -> C# property Data
    [JsonPropertyName("inode")]
    public string? Inode { get; set; }

    // Maps JSON key "data" -> C# property Data
    [JsonPropertyName("children")]

    //Children is an object, not an array
    public Dictionary<string, WegSubnavNodeRaw>? Children { get; set; }
}