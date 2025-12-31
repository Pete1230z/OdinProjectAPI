/*
PURPOSE:
These helper methods handle ODIN WEG "images" data, which is returned by GraphQL
as a JSON string rather than a structured object.
*/
using OdinProjectAPI.GraphQL;
using System.Text.Json;

//Documenation for namespaces: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/namespaces
namespace OdinProjectAPI.Output;

public sealed class WegImageParser
{
    private const string OdinBaseUrl = "https://odin.tradoc.army.mil";

    // Parses the raw JSON string returned by GraphQL ("images") into a list of image objects
    public List<WebImageDTO> ParseImages(string? imagesRaw)
    {
        // If the field is null or empty, return an empty list (no images)
        if (string.IsNullOrWhiteSpace(imagesRaw))
            return new List<WebImageDTO>();

        // Deserialize the JSON string into a list of WebImageDTO objects
        // If deserialization fails, return an empty list instead of null
        return JsonSerializer.Deserialize<List<WebImageDTO>>(imagesRaw)
            ?? new List<WebImageDTO>();
    }

    // Converts an ODIN image path into a full, downloadable URL
    public string BuildAbsoluteUrl(string? relativeUrl)
    {
        // If the URL is null or empty, return an empty string
        if (string.IsNullOrWhiteSpace(relativeUrl))
            return string.Empty;

        // If the URL already starts with http, it is already a full URL
        if (relativeUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            return relativeUrl;

        // Otherwise, prepend the ODIN base domain to create a full URL
        return OdinBaseUrl + relativeUrl;
    }

}