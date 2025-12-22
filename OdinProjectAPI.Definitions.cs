/* PURPOSE:
   This file defines data transfer objects (DTOs) that model the structure of
   GraphQL responses returned by the ODIN Content API.

   It provides a generic wrapper for the GraphQL "data" envelope and specific
   DTOs for individual query result shapes.

   This file is part of the Definitions layer and is consumed by Program.cs
   and deserialization logic when converting GraphQL JSON responses into
   strongly-typed C# objects.
*/


using System.Text.Json.Serialization;

//Documenation for namespaces: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/namespaces
namespace OdinProjectAPI.Definitions;

// GENERIC GRAPHQL RESPONSE WRAPPER

//This class represents the OUTER GraphQL response structure.
// GraphQL responses always look like:
// {
//   "data": { ... }
// }
// The <T> means this class can wrap ANY type of data.
//Documentation: https://spec.graphql.org/October2021/#sec-Overview
public sealed class GraphQLRespone<T>
{
    // Maps JSON key "data" -> C# property Data
    [JsonPropertyName("data")]

    // This property maps to the "data" field in the JSON response.
    // The type T will be replaced with a real type at runtime.
    //Generics Documentation: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/generics
    public T? Data { get; set; }
}

// SPECIFIC DATA SHAPE FOR __typename

// This class represents the INNER object:
// { "__typename": "Query" }
public sealed class TypenameData
{
    // Maps JSON key "__typename" -> C# property __typename
    [JsonPropertyName("__typename")]

    // This property name matches the JSON key exactly.
    // "__typename" is a special GraphQL meta-field.
    //Documentation: https://spec.graphql.org/October2021/#sec-Type-Name-Introspection
    public string? __typename { get; set; }
}