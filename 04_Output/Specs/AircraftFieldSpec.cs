namespace OdinProjectAPI.Output;

public static class AircraftFieldSpec
{
    public static readonly string[] Columns =
    {
        "Name","Type","ImageUrl", "MaxSpeed","CruiseSpeed","Endurance","Ceiling","Range","Crew"
    };

    // Use OrdinalIgnoreCase because ODIN section/property names are human-authored
    // and inconsistently cased across assets (e.g. "Range", "range", "RANGE").
    // This prevents silent lookup failures during extraction.
    public static readonly Dictionary<string, string[]> Fields = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Name"] = new[] { "System::Name" },
        ["Type"] = new[] { "System::Type" },

        ["MaxSpeed"] = new[]
        {
            "Automotive::Maximum Speed",
            "Automotive::Speed, Maximum"
        },

        ["CruiseSpeed"] = new[]
        {
            "Automotive::Cruise Speed",
            "Automotive::Cruising Speed"
        },

        ["Endurance"] = new[]
        {
            "Automotive::Endurance"
        },

        ["Ceiling"] = new[]
        {
            "Automotive::Service Ceiling",
            "Automotive::Ceiling",
            "Automotive::Maximum Altitude"
        },

        // Combined Range
        ["Range"] = new[]
        {
            "Automotive::Maximum Range",
            "Automotive::Ferry Range",
            "Automotive::Maximum Distance",
            "System::Maximum Range",
            "System::Range"
        },

        ["Crew"] = new[]
        {
            "System::Crew"
        }
    };
}
