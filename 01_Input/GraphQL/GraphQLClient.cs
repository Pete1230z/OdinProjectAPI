/* PURPOSE:
   This file defines a GraphQL client responsible for executing GraphQL-over-HTTP
   requests against the ODIN Content API.

   It encapsulates HTTP POST request creation, JSON serialization of GraphQL queries,
   request execution, and basic error handling.

   This file is part of the Services (Transport/Protocol) layer and is consumed by
   Program.cs to execute GraphQL queries and retrieve raw JSON responses.
*/


//This namespace provides classes for character encoding (like ASCII, UTF-8, UTF-16) and helper classes for manipulating strings efficiently, such as StringBuilder.
using System.Text;

//This is the built -in, high - performance library in modern .NET for handling JavaScript Object Notation (JSON) data. It includes the JsonSerializer class, which allows developers to:
//Serialize: Convert a C# object into a JSON string or stream.
//Deserialize: Convert a JSON string or stream back into a C# object.
using System.Text.Json;

//Documenation for namespaces: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/namespaces
namespace OdinProjectAPI.GraphQL;

public sealed class GraphQLClient
{
    // SECTION 1: DEPENDENCIES / STATE

    // A private field that holds the HttpClient instance.
    // - 'HttpClient' is the TYPE
    // - '_http' is the FIELD NAME
    // - 'readonly' means it can only be assigned in the constructor
    private readonly HttpClient _http;

    // A private field that stores the GraphQL endpoint URL
    private readonly string _endpoint;

    // SECTION 2: CONSTRUCTION / SETUP

    //Constructor: runs when a new GraphQLClient is created
    public GraphQLClient(HttpClient http, string endpoint)
    {
        //Assigns incoming HttpClient to the private field
        _http = http;

        //Assign the endpoint URL to the private field
        _endpoint = endpoint;
    }

    // SECTION 3: EXECUTE GRAPHQL REQUEST

    // Sends a GraphQL query to the server and returns the raw JSON response.
    public async Task<string> ExecuteRawAsync(string query, object variables)
    {
        // 3A: BUILD GRAPHQL REQUEST BODY

        // Create an object that matches the GraphQL HTTP payload shape:
        // { "query": "<graphql query string>" }
        var payload = new {query, variables};

        // 3B: SERIALIZE PAYLOAD TO JSON

        // Serializes the C# object into a JSON string.
        // This uses System.Text.Json, Microsoft's built-in JSON library.
        var json = JsonSerializer.Serialize(payload);

        // 3C: PREPARE HTTP CONTENT

        // Serializes the C# object into a JSON string.
        // This uses System.Text.Json, Microsoft's built-in JSON library.
        //Using implements IDisposable which releases unamanaged resources https://learn.microsoft.com/en-us/dotnet/api/system.idisposable?view=net-10.0
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        // 3D: SEND HTTP REQUEST

        // Sends an HTTP POST request to the GraphQL endpoint.
        // - POST is required by GraphQL when sending a body
        // - await pauses execution until the response is received
        using var response = await _http.PostAsync(_endpoint, content);

        // 3E: READ HTTP RESPONSE

        // Checks if the HTTP status code is NOT in the 200–299 range.
        // GraphQL servers may return errors with 400 or 500 codes.
        var body = await response.Content.ReadAsStringAsync();

        // 3F: HANDLE ERRORS

        if (!response.IsSuccessStatusCode)
            throw new Exception($"GraphQL failed: {(int)response.StatusCode }\n{body}");

        return body;
    }
}

