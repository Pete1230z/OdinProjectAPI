using System.Text.Json;

namespace OdinProjectAPI.WegSubnav;

//Docs on Static Classes: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/static-classes-and-static-class-members
public static class WegCategoryCacheReader
{
    // Loads the normalized WEG category cache from disk and converts it
    // back into strongly-typed objects the application can work with.
    public static async Task<WegCategoryCache> LoadAsync(string path = "weg-categories.json")
    {
        var json = await File.ReadAllTextAsync(path);

        //JsonSerializer Docs: https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializer?view=net-10.0
        var cache = JsonSerializer.Deserialize<WegCategoryCache>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive= true });

        return cache ?? throw new InvalidCastException("Failed to deserialize weg-categories.json");
    }

    // Recursively searches the category tree for a node with a matching Variable.
    // This allows locating any category anywhere in the hierarchy.
    public static WegCategoryNode? FindByVariable(WegCategoryNode node, string variable)
    {
        if (node.Variable == variable) return node;

        foreach (var child in node.Children)
        {
            var found = FindByVariable(child, variable);
            if (found != null) return found;
        }

        return null;
    }

    // Recursively searches the category tree for a node with a matching Variable.
    // This allows locating any category anywhere in the hierarchy.
    public static List<DropdownOption> ToDropdownOptions(WegCategoryNode parent)
    {
        return parent.Children
            .Select(c => new DropdownOption { Label = c.DisplayName, Value = c.Variable})
            .OrderBy(o => o.Label)
            .ToList();
    }
}