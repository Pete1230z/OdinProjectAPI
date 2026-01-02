namespace OdinProjectAPI.Output.Specs;

public static class TankFieldSpec
{
    public static readonly string[] Columns =
    {
        "Name",
        "Type",
        "ImageUrl",
        "Crew",
        "MaxSpeed",
        "VehicleRange",
        "Armor",
        "Caliber",
        "RateOfFire",
        "MaxRange"
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

            // Mobility
            ["MaxSpeed"] = new[]
            {
                "Automotive::Speed, Maximum Road",
                "Automotive::Maximum Speed"
            },

            ["VehicleRange"] = new[]
            {
                "Automotive::Cruising Range",
                "Automotive::Range"
            },

            // Protection fallback chain
            ["Armor"] = new[]
            {
                "Protection::Hull Armor",
                "Protection::Turret Armor",
                "Protection::Armor"
            },

            // Main gun concept (normalized to System level)
            ["Caliber"] = new[]
            {
                "System::Caliber",
                "Ammunition::Caliber"
            },

            ["RateOfFire"] = new[]
            {
                "System::Max Rate of Fire",
                "System::Maximum Rate of Fire",
                "System::Sustain Rate of Fire",
                "System::Rate of Fire"
            },

            // MaxRange = any firing/range concept
            ["MaxRange"] = new[]
            {
                "System::Maximum Firing Range",
                "System::Effective Firing Range",
                "System::Maximum Effective Range"
            },

        };
}
