namespace OdinProjectAPI.Output.Specs;

public static class InfantryVehiclesFieldSpec
{
    public static readonly string[] Columns =
    {
        "Name",
        "Type",
        "ImageUrl",
        "Crew",
        "MaxSpeed",
        "Range",
        "Armor",
        "Caliber",
        "RateOfFire",
        "MaxRange",
        "ArmorPenetration"
    };

    public static readonly Dictionary<string, string[]> Fields =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["Name"] = new[]
            {
                "System::Name"
            },

            ["Type"] = new[]
            {
                "System::Type"
            },

            ["Crew"] = new[]
            {
                "System::Crew"
            },

            // Speed: multiple labels exist; prefer "Maximum Road"
            ["MaxSpeed"] = new[]
            {
                "Automotive::Speed, Maximum Road",
                "Automotive::Maximum Speed",
                "Automotive::Speed, Maximum"
            },

            // Range: treat all range-like fields as Range (your rule)
            ["Range"] = new[]
            {
                "Automotive::Maximum Range",
                "Automotive::Cruising Range",
                "Automotive::Range",
                "Automotive::Endurance"
            },

            // Armor: your preferred fallback chain
            ["Armor"] = new[]
            {
                "Protection::Hull Armor",
                "Protection::Turret Armor",
                "Protection::Armor"
            },

            // Weapons (minimal, normalized)
            // Note: weapon details are often duplicated across System/Main Gun/Main Weapon System,
            // so we target the most common location first.
            ["Caliber"] = new[]
            {
                "System::Caliber",
                "Ammunition::Caliber"
            },

            ["RateOfFire"] = new[]
            {
                "System::Max Rate of Fire",
                "System::Rate of Fire"
            },

            // MaxRange = any range concept
            ["MaxRange"] = new[]
            {
                "System::Maximum Firing Range",
                "System::Effective Firing Range",
                "System::Maximum Effective Range"
            },

            ["ArmorPenetration"] = new[]
            {
                "Ammunition::Armor Penetration"
            }
        };
}
