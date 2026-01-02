using OdinProjectAPI.WegSubnav;

namespace OdinProjectAPI.Output;

// Enum defines the bundle OPTIONS a user (or code) can select.
// IMPORTANT: enums do NOT contain logic or data — they are just names.
public enum WeaponBundleKey
{
    DirectFire,         // Rifles, Machine Guns, etc.
    IndirectFire,       // Mortars, Grenade Launchers
    RocketsAndGrenades, // Rocket Launchers / AT rockets / Grenades
    AllInfantryWeapons  // A COMPOSED bundle (DF + IDF + Rockets)
}

public static class WeaponSystemBundles
{
    // This dictionary defines what EACH bundle means in terms of CATEGORY LABELS.
    // These are human-readable labels from the WEG subnav tree (not variables).
    // We use labels instead of variables because variables can change,
    // while labels are what users actually see.

    private static readonly Dictionary<WeaponBundleKey, List<string>> Labels =
        new()
        {
            // Direct fire infantry weapons
            [WeaponBundleKey.DirectFire] = new()
            {
                "Rifles",
                "Machine Guns",
                "Flamethrowers",
                "Recoilless Guns",
                "Handguns",
                "Submachine Guns (SMG)",
                "Shotguns"
            },

            // Indirect fire infantry weapons
            [WeaponBundleKey.IndirectFire] = new()
            {
                "Mortars",
                "Grenade Launchers"
            },

            // Rocket-based infantry weapons
            [WeaponBundleKey.RocketsAndGrenades] = new()
            {
                "Rocket Launchers",
                "Anti-Tank Guided Missiles (ATGM)",
                "Grenades"
            }
        };

    // Expands one or more bundle keys into CATEGORY VARIABLES
    // because Lucene filtering requires category variables, not labels.
    public static List<string> Expand(WegCategoryNode selectedDomainNode, params WeaponBundleKey[] bundles)
    {
        // Use a HashSet to avoid duplicate variables
        // (e.g. if multiple bundles include the same category)
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var bundle in bundles)
        {
            // SPECIAL CASE:
            // AllInfantryWeapons is NOT a real category.
            // It is defined as the UNION of the other supported bundles.
            if ( bundle == WeaponBundleKey.AllInfantryWeapons)
            {
                // AllInfantryWeapons = DirectFire + IndirectFire + RocketsAndGrenades
                AddBundle(selectedDomainNode, set, WeaponBundleKey.DirectFire);
                AddBundle(selectedDomainNode, set, WeaponBundleKey.IndirectFire);
                AddBundle(selectedDomainNode, set, WeaponBundleKey.RocketsAndGrenades);

                continue;
            }

            // Normal case: expand a single bundle
            AddBundle(selectedDomainNode, set, bundle);
        }

        // Return the resolved category VARIABLES as a list
        return set.ToList();
    }

    private static void AddBundle(WegCategoryNode domainNode, HashSet<string> set, WeaponBundleKey bundle)
    {
        // Look up the labels for this bundle
        if (!Labels.TryGetValue(bundle, out var labels)) return;

        // Resolve each label into a category variable
        foreach (var label in labels)
        {
            // Convert label -> variable dynamically from the category tree
            // because we do NOT want to hardcode variables
            var variable = WegCategoryResolver.ResolveVariableByLabel(domainNode, label);

            set.Add(variable);
        }
    }
}