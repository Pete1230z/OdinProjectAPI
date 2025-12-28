namespace OdinProjectAPI.WegSubnav;

//Hold user's dropdowns in a single object
public sealed class WegFilterCriteria
{
    // Example: "land-f5e1db"
    public string? DomainVariable { get; set; }

    // Example: "infantry-weapons-6965ab"
    public string? WeaponSystemTypeVariable { get; set; }

    // Origin filters (from GraphQL), not from DotCMS subnav
    public string? OriginVariable { get; set; }

    // Tier range mapped to dateOfIntroduction (Lucene range)
    public string? TierKey { get; set; }
}