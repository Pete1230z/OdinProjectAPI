/*
WegSectionsParser.Parse(sectionsRaw) produces objects
Get a List<WegSection> like:
WegSection.Name = "Automotive"
Properties[0].Name = "Maximum Speed"
Properties[0].Value = "2,200"
Properties[0].Units = "km/h"
Properties[1].Name = "Service Ceiling"
Properties[1].Value = "60,000"
Properties[1].Units = "ft"

WegSectionIndexBuilder.BuildIndex(sections) flattens sections from Wegsections parser into a dictionary
It loops through each section/property and builds keys:
Key: "Automotive::Maximum Speed"
Value: "2,200 km/h"(CombineValueUnits did this)
Key: "Automotive::Service Ceiling"
Value: "60,000 ft"

WegCardFieldExtractor.Extract(card, fieldSpec) applies fallback rules
Aircraft spec defines:
MaxSpeed fallbacks:
"Automotive::Maximum Speed"
"Automotive::Speed, Maximum"
Ceiling fallbacks:
"Automotive::Service Ceiling"
"Automotive::Ceiling"
"Automotive::Maximum Altitude"
The extractor does:
For MaxSpeed
Check index["Automotive::Maximum Speed"] → found "2,200 km/h"
Stop (first match wins)
Output: MaxSpeed = "2,200 km/h"
For Ceiling
Check index["Automotive::Service Ceiling"] → found "60,000 ft"
Stop
Output: Ceiling = "60,000 ft"
*/

using OdinProjectAPI.Inventory;


namespace OdinProjectAPI.Output;

public static class WegSectionIndexBuilder
{
    // Builds a lookup like:
    // "System::Maximum Range" -> "500 km"
    //
    // because scanning every section and property repeatedly would be slow
    // and complicated when applying fallback rules
    public static Dictionary<string, string> BuildIndex(List<WegSection> sections)
    {
        // Create a dictionary to hold the lookup
        // Case-insensitive because ODIN section/property names are inconsistent
        // (e.g. "Range" vs "range")
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // Loop through all top level sections in the card
        foreach (var section in sections)
        {
            // Add this section and anything nested under it into the dictionary
            AddSection(dict, section);
        }

        return dict;
    }

    private static void AddSection(Dictionary<string, string> dict, WegSection section)
    {
        var sectionName = Clean(section.Name);

        // Add this section and anything nested under it into the dictionary
        if (string.IsNullOrWhiteSpace(sectionName)) {
            sectionName = "Unnamed Section.";
        };

        // Index all properties directly on this section
        // Example key: "System::Maximum Range"
        if (section.Properties != null)
        {
            foreach (var prop in section.Properties)
            {
                var propName = Clean(prop.Name);

                if (string.IsNullOrWhiteSpace(propName)) {
                    propName = "Unnamed Property.";
                }

                // Combine value and units into one display string
                // Example: "500" + "km" -> "500 km"
                var value = CombineValueUnits(prop.Value, prop.Units);

                if (string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                var key = ($"{sectionName}::{propName}");

                // Only store the first value we see for this key
                // because ODIN sometimes repeats properties
                // and we want deterministic, stable output
                if (!dict.ContainsKey(key))
                {
                    dict[key] = value;
                }
            }
        }

        // Recurse into subsections (if any)
        // because ODIN allows nested sections
        if (section.Sections != null)
        {
            foreach (var sub in section.Sections)
            {
                AddSection(dict, sub);
            }
        }
    }

    // Recurse into subsections (if any)
    // because ODIN allows nested sections
    private static string CombineValueUnits(string? value, string? units)
    {
        var v = Clean(value);

        if (string.IsNullOrWhiteSpace(v)) return "";

        var u = Clean(units);

        // If there are no units, return just the value
        // otherwise return "value units"
        return string.IsNullOrWhiteSpace(u) ? v : $"{v} {u}";
    }

    // Trims whitespace and converts null to empty string
    // because ODIN data is messy and null-safe handling is required
    private static string Clean(string? value) => (value ?? "").Trim();
}