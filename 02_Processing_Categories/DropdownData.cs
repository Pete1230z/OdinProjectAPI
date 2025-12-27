namespace OdinProjectAPI.WegSubnav;

//Simple dropdown option(Label shown to end user, value used internally for filtering)
public sealed class DropdownOption
{
    public string Label { get; set; } = "";
    public string Value { get; set; } = "";
}