/*
PURPOSE:
This file is used to help determine the shape of the DotCMS subnavigation endpoint so we can then create dropdowns on a front end for users"
*/
using System.Text.Json;
namespace OdinProjectAPI.WegSubnav;

/*One-off utility used to retrieve and inspect the raw WEG category tree from the ODIN DotCMS subnavigation endpoint.*/
//Docs on Static Classes: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/static-classes-and-static-class-members
public static class WegSubnavStructure
{

    //Downloads the WEG subnav JSON, prints basic diagnostics
    /*Async: The method contains awaitable asynchronous operations. The compiler rewrites the method into a state machine.Execution can pause without blocking a thread
     The HTTP request is sent. When the response arrives, execution resumes
    */
    //Async Return Types: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/async-return-types
    public static async Task WegSubnavAsync()
    {
        // Public ODIN DotCMS endpoint that returns WEG category metadata
        var url = "https://odin.tradoc.army.mil/dotcms/api/subnav/weg";

        // HttpClient is responsible for sending HTTP requests (POST, GET, etc.).
        // This object manages network connections and should be reused rather than recreated repeatedly.
        //Documentation https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-10.0
        using var http = new HttpClient();

        //Docs: https://learn.microsoft.com/dotnet/api/system.net.http.httpclient.getstringasync
        var json = await http.GetStringAsync(url);

        Console.WriteLine($"Length: {json.Length} chars");
        Console.WriteLine("First 3000 Characters");
        Console.WriteLine(json[..Math.Min(3000, json.Length)]);

        //Docs: https://learn.microsoft.com/dotnet/api/system.io.file.writealltextasync
        await File.WriteAllTextAsync("weg-subnav-raw.json", json);
        Console.WriteLine("Saved: weg-subnav-raw.json");
    }

    // Reads the previously saved subnav JSON file and inspects its structure.
    // This proves our DTOs match the real response shape and that traversal works.
    public static async Task InspectParsedAsync()
    {
        // Read the raw JSON captured earlier
        var json = await File.ReadAllTextAsync("weg-subnav-raw.json");

        //Deserialize into DTOs that mirror the API response
        var data = JsonSerializer.Deserialize<WegSubnavResponse>(json, new JsonSerializerOptions {  PropertyNameCaseInsensitive = true });

        //Check root node
        Console.WriteLine($"Root Node Name: {data?.Content?.Name}");

        //Check top level children count
        Console.WriteLine($"Top Level children count: {data?.Content?.Children?.Count ?? 0}");

        //!. documentation: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/null-forgiving
        var domainNode = data!.Content!.Children!["domain"];

        Console.WriteLine("Domain Children");

        /*Tuple deconstruction. 
         _ means “ignore this value”
        childNode captures the dictionary value

        Dictionary<string, WegSubnavNodeRaw>
        .Key → the dictionary key (e.g., "sea-637ec6")
        .Value → the actual child node object
        Docs: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/functional/deconstruct
        */
        foreach (var (_, childNode) in domainNode.Children)
        {
            Console.WriteLine($"{childNode.Name} | variable {childNode.Variable}");
        }
    }
}