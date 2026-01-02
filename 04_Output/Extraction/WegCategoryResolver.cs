using OdinProjectAPI.WegSubnav;

namespace OdinProjectAPI.Output;

public static class WegCategoryResolver
{
    // Example flow:
    // startNode = "Land"
    // label = "Mortars"
    //
    // This method will walk the category tree like:
    // Land -> Infantry Weapons -> Mortars
    //
    // and return the WegCategoryNode whose DisplayName == "Mortars".
    public static WegCategoryNode FindDescendantByLabel(WegCategoryNode startNode, string label)
    {
        var target = (label ?? "").Trim();
        if (string.IsNullOrWhiteSpace(target))
        {
            throw new ArgumentException("Label cannot be empty.", nameof(label));
        }

        // Stack is used to walk the category tree without recursion.
        // Each node pushed onto the stack will be checked.
        var stack = new Stack<WegCategoryNode>();

        // Start searching from the selected domain node (e.g. "Land")
        stack.Push(startNode);

        while (stack.Count > 0)
        {
            // Take the next node to inspect
            var node = stack.Pop();

            // If the display label matches (case-insensitive), return it
            if (string.Equals(node.DisplayName?.Trim(), target, StringComparison.OrdinalIgnoreCase)) return node;

            // Otherwise, add all children to the stack
            // so they will be checked next
            foreach (var child in node.Children)
            {
                stack.Push(child);
            }
        }

        throw new InvalidOperationException($"Category label not found under selected domain: {label}");
    }

    // Converts a label (e.g. "Mortars") into a category variable
    // (e.g. "mortars-3a9f21") that Lucene queries require.
    public static string ResolveVariableByLabel(WegCategoryNode startNode, string label)
    {
        // Find the node by walking the tree
        var node = FindDescendantByLabel(startNode, label);

        if (string.IsNullOrWhiteSpace(node.Variable))
        {
            throw new InvalidOperationException($"Resolved node has no variable (can not be queried).");
        }

        // Return the variable used in Lucene queries
        return node.Variable;
    }
}