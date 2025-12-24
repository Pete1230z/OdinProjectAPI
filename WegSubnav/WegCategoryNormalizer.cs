/*
PURPOSE:
Transforms raw DotCMS WEG subnavigation DTOs into normalized category models
used by the application.

The normalizer:
- Converts API-shaped data into a clean internal structure
- Enforces required fields (e.g., query variables)
- Injects parent-child relationships explicitly
- Recursively processes the entire category tree

This class contains no I/O and no UI logic.
It exists solely to translate external data into a reliable internal form.
*/

namespace OdinProjectAPI.WegSubnav;

//Docs on Static Classes: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/static-classes-and-static-class-members
public static class WegCategoryNormalizer
{
    public static WegCategoryNode? Normalize(WegSubnavNodeRaw raw, string? parentVariable = null)
    {
        var displayName = raw.Name?.Trim();
        if (string.IsNullOrWhiteSpace(displayName))
            displayName = "(Unnamed)";

        //Variable is what is filtered on, if missing node is not usable
        var variable = raw.Variable?.Trim();
        if (string.IsNullOrWhiteSpace(variable))
            variable = null;

        var node = new WegCategoryNode
        {
            DisplayName = displayName,
            Variable = variable,
            ParentVariable = parentVariable
        };

        if (raw.Children is null || raw.Children.Count == 0)
        {
            return node;
        }

        foreach (var (_, childRaw) in raw.Children)
        {
            var childNode = Normalize(childRaw, variable);
            if (childNode != null)
            {
                node.Children.Add(childNode);
            }
        }

        return node;
    }
}