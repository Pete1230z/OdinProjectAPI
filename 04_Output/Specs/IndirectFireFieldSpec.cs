namespace OdinProjectAPI.Output.Specs;

public static class IndirectFireFieldSpec
{
    // Columns we will output to Excel for the IDF bundle
    public static readonly string[] Columns =
    {
        "Name",
        "Type",
        "ImageUrl",
        "Caliber",
        "RateOfFire",
        "EffectiveRange",
        "MaxRange",
        "KillRadius",
        "ArmorPenetration"
    };

    // Column -> fallback lookup keys ("Section::Property")
    // Use OrdinalIgnoreCase because ODIN casing is inconsistent
    public static readonly Dictionary<string, string[]> Fields =
        new(StringComparer.OrdinalIgnoreCase)
        {
            // Basic identity fields
            ["Name"] = new[]
            {
                "System::Name"
            },

            ["Type"] = new[]
            {
                "System::Type"
            },

            // Caliber is sometimes on System, sometimes on Ammunition
            ["Caliber"] = new[]
            {
                "System::Caliber",
                "Mortar::Caliber",
                "Ammunition::Caliber"
            },

            // Rate of Fire shows up primarily on System
            ["RateOfFire"] = new[]
            {
                "System::Rate of Fire",
                "Mortar::Rate of Fire, Burst"
            },

            // Effective range appears under multiple similar names
            ["EffectiveRange"] = new[]
            {
                "System::Maximum Effective Range",
                "System::Maximum Effective Firing Range",
                "System::Effective Firing Range",
                "System::Minimum Effective Range"
            },

            // Max range is often stored as "Maximum Firing Range" or "Maximum Range"
            ["MaxRange"] = new[]
            {
                "System::Maximum Firing Range",
                "System::Maximum Range",
                "Mortar::Range"
            },

            // Rare, but useful for explosive IDF systems; expect blanks
            ["KillRadius"] = new[]
            {
                "System::Killing Radius"
            },

            // Rare in IDF (seen under Ammunition); expect blanks
            ["ArmorPenetration"] = new[]
            {
                "Ammunition::Armor Penetration"
            }
        };
}
