namespace OdinProjectAPI.Output.Specs;

public static class AirDefenseFieldSpec
{
    public static readonly string[] Columns =
    {
        "Name",
        "Type",
        "ImageUrl",
        "Crew",
        "MissileName",
        "GuidanceSystem",
        "Warhead",
        "EffectiveRange",
        "MaxRange",
        "MaxAltitude",
        "ReactionTime",
        "EmplacementTime",
        "DisplacementTime",
        "VehicleRange"
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

            // Missile identity (often present)
            ["MissileName"] = new[]
            {
                "Missile::Name"
            },

            // Guidance exists on both System and Missile
            ["GuidanceSystem"] = new[]
            {
                "Missile::Guidance System",
                "System::Guidance System",
                "Fire Control System::Computerized FCS" // last-resort indicator of guidance/FC capability
            },

            // Warhead summary
            ["Warhead"] = new[]
            {
                "Missile::Warhead Type",
                "Missile::Warhead Weight",
                "System::Note"
            },

            // EffectiveRange = only maximum effective concepts (often blank for SAMs)
            ["EffectiveRange"] = new[]
            {
                "System::Maximum Effective Range"
            },

            // MaxRange = any range concept
            ["MaxRange"] = new[]
            {
                "Missile::Operational Range",
                "System::Maximum Range",
                "System::Minimum Range" // only used if nothing else exists; will look odd but avoids blank
            },

            // MaxAltitude = any altitude ceiling concept
            ["MaxAltitude"] = new[]
            {
                "Missile::Maximum Altitude",
                "System::Maximum Altitude"
            },

            ["ReactionTime"] = new[]
            {
                "System::Reaction Time"
            },

            ["EmplacementTime"] = new[]
            {
                "System::Emplacement Time"
            },

            ["DisplacementTime"] = new[]
            {
                "System::Displacement Time"
            },

            ["VehicleRange"] = new[]
            {
                "Automotive::Maximum Range",
                "Automotive::Cruising Range"
            },
        };
}
