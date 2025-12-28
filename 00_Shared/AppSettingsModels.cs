/* PURPOSE:
    This file defines configuration data transfer objects (DTOs) used to bind values
    from appsettings.json into strongly-typed C# objects at application startup.

    This file is part of the Configuration layer and is consumed by Program.cs
    during application initialization.
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

    public sealed class AppSettings
    {
        //References OdinSettings instead of a general other type such as a string
        //Odin is the property name, which is used as the "lookup" to find the correct JSON object
        public OdinSettingsDTO Odin { get; set; } = new();
        public WegSettings Weg { get; set; } = new();
        public string? OutputFolder { get; set; }
    }
}