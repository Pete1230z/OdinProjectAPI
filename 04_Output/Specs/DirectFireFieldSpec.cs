namespace OdinProjectAPI.Output.Specs;

public static class DirectFireFieldSpec
{
    // Output columns for Direct Fire weapons (rifles + machine guns).
    public static readonly string[] Columns =
    {
        "Name",
        "Type",
        "ImageUrl",
        "Caliber",
        "RateOfFire",
        "MaxRange"
    };

    // Column -> fallback keys ("Section::Property")
    // Case-insensitive because ODIN casing varies.
    public static readonly Dictionary<string, string[]> Fields =
        new(StringComparer.OrdinalIgnoreCase)
        {
            // Identity
            ["Name"] = new[]
            {
                "System::Name"
            },

            ["Type"] = new[]
            {
                "System::Type",
                "System::Primary Function / Type"
            },

            // Caliber appears both on System and Ammunition
            ["Caliber"] = new[]
            {
                "System::Caliber",
                "Ammunition::Caliber",
                "Ammunition (Option 1)::Caliber",
                "Ammunition (Option 2)::Caliber",
                "Ammunition (Option 3)::Caliber"
            },

            // Rate of fire is consistently available
            ["RateOfFire"] = new[]
            {
                "System::Rate of Fire",
                "System::Cyclic Rate of Fire",
                "System::Rate of Fire, Practical"
            },

            // Max range (absolute)
            ["MaxRange"] = new[]
            {
                "System::Maximum Firing Range",
                "System::Maximum Range",
                "System::Maximum Effective Firing Range",
                "System::Effective Firing Range",
                "System::Effective Firing Range, Optical Sight",
                "System::Effective Firing Range, Iron Sight"
            }
        };
}
