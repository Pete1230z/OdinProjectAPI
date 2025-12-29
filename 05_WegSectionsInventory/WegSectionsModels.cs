namespace OdinProjectAPI.Inventory;

public sealed class WegSection
{
    public string? Name { get; set; }

    public List<WegSectionProperty> Properties { get; set; } = new();

    //List<T> is a reference type and would be null unless you initialized
    //= new() creates an empty list by default, so the property is always usable.
    public List<WegSection> Sections { get; set; } = new();

}

public sealed class WegSectionProperty
{
    public string Name { get; set; } = "";

    public string Value { get; set; } = "";

    public string Units { get; set; } = "";
}