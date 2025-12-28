/* PURPOSE:
    This file defines configuration data transfer objects (DTOs) used to bind values
    from appsettings.json into strongly-typed C# objects at application startup.

    This file is part of the Configuration layer and is consumed by Program.cs
    during application initialization.

    The property names tell the binder where to look, and then bind it into the property
    This tells the binder:

    Look for a config section called "Odin"

    Bind it into OdinSettingsDTO

    Look for a section called "Weg"

    Bind it into WegSettings

    Look for "OutputFolder"

    Assign it to the string
*/

namespace OdinProjectAPI.Configuration
{
    //DTO (DATA TRANSFER OBJECT) A class used only to carry data.
    public sealed class OdinSettingsDTO
    {
        //get and set accessors perform no other operation than setting or retrieving a value
        public string? ForceStructureAPI { get; set; }
        public string? DISEnumerationAPI { get; set; }
        public string? GraphQLEndPoint { get; set; }

    }

    public sealed class WegSettings
    {
        public List<WegTierDefinition> Tiers { get; set; }
    }

    public sealed class WegTierDefinition
    {
        //Docs Reference Type: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/reference-types
        public string Key { get; set; } = "";
        public string Label { get; set; } = "";

        //Docs Value Type: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/value-types
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }

    public sealed class AppSettings
    {
        //References OdinSettings instead of a general other type such as a string
        //Odin is the property name, which is used as the "lookup" to find the correct JSON object
        public OdinSettingsDTO Odin { get; set; } = new();
        public WegSettings Weg { get; set; } = new();
        public string? OutputFolder { get; set; }
    }
}