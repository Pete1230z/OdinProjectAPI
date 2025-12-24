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