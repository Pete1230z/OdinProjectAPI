namespace OdinProjectAPI.Output.Specs;

public static class ArtilleryFieldSpec
{
    public static readonly string[] Columns =
    {
        "Name",
        "Type",
        "ImageUrl",
        "Caliber",
        "RateOfFire",
        "MaxRange",
        "EmplacementTime",
        "DisplacementTime",
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

            // MaxRange = any range concept
            ["MaxRange"] = new[]
            {
                "System::Maximum Firing Range",
                "System::Maximum Effective Range",
                "Ammunition (Option 2)::Maximum Effective Range"
            },

            ["EmplacementTime"] = new[]
            {
                "System::Emplacement Time"
            },

            ["DisplacementTime"] = new[]
            {
                "System::Displacement Time"
            }
        };
}
