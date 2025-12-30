using System.ComponentModel;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace OdinProjectAPI.Inventory;


public sealed class WegSectionsInventoryComparer
{
    // Counts how often each TOP-LEVEL section appears
    // Example: "System" -> 312
    public Dictionary<string, int> SectionCounts { get; } = new(StringComparer.OrdinalIgnoreCase);

    // Counts subsection relationships
    // Example: "Main Weapon System > Ammunition" -> 97
    public Dictionary<string, int> SubsectionCounts { get; } = new(StringComparer.OrdinalIgnoreCase);

    // Counts properties per section
    // Example: "System :: Crew" -> 280
    public Dictionary<string, int> PropertyCounts { get; } = new(StringComparer.OrdinalIgnoreCase);

    // Called once per card; processes all its top-level sections
    public void AddCardSections(List<WegSection> sections)
    {
        foreach (var section in sections)
        {
            // parentName = null means this is a top-level section
            AddSectionRecursive(section, parentName: null);
        }
    }

    // Walks a section and any nested subsections
    private void AddSectionRecursive(WegSection section, string? parentName)
    {
        //Normalize Section Name
        var sectionName = (section.Name ?? "").Trim();
        if (string.IsNullOrWhiteSpace(sectionName)) sectionName = "Unnamed Section";

        //This counts top level headers
        if (parentName == null) Increment(SectionCounts, sectionName);

        // Count all properties directly under this section
        if (section.Properties != null && section.Properties.Count > 0)
        {
            foreach (var prop in section.Properties)
            {
                var propName = (prop.Name ?? "").Trim();
                if (string.IsNullOrWhiteSpace(propName)) propName = "Unnamed Property";

                // Key format: "Section :: Property"
                Increment(PropertyCounts, $"{section.Name} :: {propName}");
            }
        }

        // Handle nested subsections (if any)
        if (section.Sections != null && section.Sections.Count > 0)
        {
            foreach (var sub in section.Sections)
            {
                var subName = (sub.Name ?? "").Trim();
                if (string.IsNullOrWhiteSpace(subName)) subName = "Unnamed SubSection";

                // Key format: "ParentSection > SubSection"
                Increment(SubsectionCounts, $"{sectionName} > {subName}");

                // Recurse into the subsection
                AddSectionRecursive(sub, parentName: sectionName);
            }
        }
    }

    // Increments a count or creates it if the key does not exist
    private static void Increment(Dictionary<string, int> dict, string key)
    {
        if (dict.TryGetValue(key, out var count))
            dict[key] = count + 1;
        else
            dict[key] = 1;
    }
}