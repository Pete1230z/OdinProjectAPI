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
    // This property name matches the JSON key exactly.
    // "__typename" is a special GraphQL meta-field.
    //Documentation: https://spec.graphql.org/October2021/#sec-Overview
    public string? __typename { get; set; }
}