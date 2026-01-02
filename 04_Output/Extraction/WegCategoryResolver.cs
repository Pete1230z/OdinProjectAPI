using OdinProjectAPI.WegSubnav;

namespace OdinProjectAPI.Output;

public static class WegCategoryResolver
{
    // Finds a node anywhere under the selected domain by its display label
    // because bundles are defined by labels, not by hardcoded category variables.
    public static WegCategoryNode FindDescendantByLabel(WegCategoryNode startNode, string label)
    {
        var target = (label ?? "").Trim();
        if (string.IsNullOrWhiteSpace(target))
        {
            throw new ArgumentException("Label cannot be empty.", nameof(label));
        }

        var stack = new Stack<WegCategoryNode>();
        stack.Push(startNode);

        while (stack.Count > 0)
        {
            var node = stack.Pop();

            if (string.Equals(node.DisplayName?.Trim(), target, StringComparison.OrdinalIgnoreCase)) return node;

            foreach (var child in node.Children)
            {
                stack.Push(child);
            }
        }

        throw new InvalidOperationException($"Category label not found under selected domain: {label}");
    }

    // Resolves the category variable for a given label
    // because Lucene queries filter on category variables.
    public static string ResolveVariableByLabel(WegCategoryNode startNode, string label)
    {
        var node = FindDescendantByLabel(startNode, label);

        if (string.IsNullOrWhiteSpace(node.Variable))
        {
            throw new InvalidOperationException($"Resolved node has no variable (can not be queried).");
        }

        return node.Variable;
    }
}