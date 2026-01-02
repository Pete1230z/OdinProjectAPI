namespace OdinProjectAPI.Output.Specs;

public static class RocketsFieldSpec
{
    // Output columns for Rocket Launchers / ATGMs / Grenades.
    public static readonly string[] Columns =
    {
        "Name",
        "Type",
        "ImageUrl",
        "Caliber",
        "MaxRange",
        "ArmorPenetration",
        "Warhead",
        "GuidanceSystem"
    };

    // Column -> fallback keys ("Section::Property")
    // Case-insensitive because ODIN casing/punctuation varies.
    public static readonly Dictionary<string, string[]> Fields =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["Name"] = new[]
            {
                "System::Name"
            },

            ["Type"] = new[]
            {
                "System::Type",
                "System::Primary Function / Type"
            },

            ["Caliber"] = new[]
            {
                "System::Caliber",
                "Dimensions::Caliber",
                "Ammunition::Caliber"
            },

            // MaxRange = ANY range concept (effective, max, operational, missile range, etc.)
            ["MaxRange"] = new[]
            {
                "System::Maximum FIring Range",   // ODIN typo observed
                "System::Maximum Firing Range",
                "System::Maximum Range",
                "System::Operational Range",
                "Missile::Range of Fire",
                "Missile::Effective Range",
                "System::Effective Firing Range"
            },

            ["ArmorPenetration"] = new[]
            {
                "System::Armor Penetration",
                "Missile::Armor Penetration",
                "Ammunition::Penetration"
            },

            ["Warhead"] = new[]
            {
                "System::Warhead",
                "Missile::Warhead Type",
                "Ammunition::Warhead",
                "Missile::Warhead Weight",
                "Dimensions::Warhead Weight"
            },

            ["GuidanceSystem"] = new[]
            {
                "System::Guidance System",
                "Missile::Guidance System",
                "Fire Control::Guidance System"
            }
        };
}
