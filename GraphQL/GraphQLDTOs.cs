/* PURPOSE:
   This file defines data transfer objects (DTOs) that model the structure of
   GraphQL responses returned by the ODIN Content API.

   It provides a generic wrapper for the GraphQL "data" envelope and specific
   DTOs for individual query result shapes.

   This file is part of the Definitions layer and is consumed by Program.cs
   and deserialization logic when converting GraphQL JSON responses into
   strongly-typed C# objects.
*/

/*
GRAPHQL DESERIALIZATION SHAPE

GraphQLResponse<T>
 └─ Data (WegCardCollectionData)
     └─ WegCardCollection (List<WegCardItem>)
         └─ WegCardItem
             ├─ Name
             ├─ Origin        (only populated if requested in query)
             ├─ Notes         (only populated if requested in query)
             ├─ ImagesRaw     (JSON string; parsed separately if requested)
             ├─ SectionsRaw   (JSON string; parsed separately if requested)
             └─ Other fields as added over time

Notes:
- GraphQL always wraps responses in a "data" object.
- DTOs are a superset of possible fields.
- Missing fields in a query simply deserialize as null.
- No errors occur if a field is not requested.
*/

using System.Text.Json.Serialization;

//Documenation for namespaces: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/namespaces
namespace OdinProjectAPI.GraphQL;

// GENERIC GRAPHQL RESPONSE WRAPPER

//This class represents the OUTER GraphQL response structure.
// GraphQL responses always look like:
// {
//   "data": { ... }
// }
// The <T> means this class can wrap ANY type of data.
//Documentation: https://spec.graphql.org/October2021/#sec-Overview
public sealed class GraphQLResponse<T>
{
    // Maps JSON key "data" -> C# property Data
    [JsonPropertyName("data")]

    // This property maps to the "data" field in the JSON response.
    // The type T will be replaced with a real type at runtime.
    //Generics Documentation: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/generics
    public T? Data { get; set; }
}

//Documentation: https://spec.graphql.org/October2021/#sec-Overview
public sealed class WegCardCollectionData
{
    /// { "data": { "wegCardCollection": [ { "name": "..." } ] } }
    [JsonPropertyName("wegCardCollection")]

    public List<WegCardItem>? WegCardCollection { get; set; }
}

public sealed class WegCardItem
{
    // Maps JSON key "data" -> C# property Data
    // This class represents the INNER object:
    // { "__typename": "Query" }
    [JsonPropertyName("name")]

    public string? Name { get; set; }

    [JsonPropertyName("origin")]
    public List<DotCategoryDTO>? Origin {  get; set; }

    [JsonPropertyName("images")]
    public string? ImagesRaw { get; set; }
}

public sealed class DotCategoryDTO
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

public sealed class WebImageDTO
{
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}