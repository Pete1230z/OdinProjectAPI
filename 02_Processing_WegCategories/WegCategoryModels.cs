/*
PURPOSE:
Defines normalized category models used internally by the application
to represent WEG categories in a predictable, UI- and filter-friendly form.

These models are derived from the raw DotCMS DTOs and:
- Enforce non-null fields where required
- Provide a stable tree structure for dropdowns
- Preserve parent/child relationships using queryable variables

These models are safe to persist, traverse, and bind to front-end controls.
*/


namespace OdinProjectAPI.WegSubnav;

//Clean node used by dropdowns for later filtering
public sealed class WegCategoryNode
{
    //Can not be null
    public string DisplayName { get; set; } = "";
    //Can not be null
    public string Variable { get; set; } = "";
    //Can be null
    public string? ParentVariable { get; set; }
    //List<T> is a reference type and would be null unless you initialized
    //= new() creates an empty list by default, so the property is always usable.
    public List<WegCategoryNode> Children { get; set; } = new();
}

//What can be stored on disk
public sealed class WegCategoryCache
{
    public DateTime RetrievedUtc { get; set; }

    //Root nodes of the normalized tree
    public List<WegCategoryNode> RootNodes { get; set; } = new();
}