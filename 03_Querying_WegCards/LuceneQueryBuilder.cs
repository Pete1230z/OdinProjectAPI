using OdinProjectAPI.Configuration;
using System.Globalization;

namespace OdinProjectAPI.WegSubnav;

//Builds a Lucene query from user selected criteria
//Docs on Static Classes: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/static-classes-and-static-class-members
public static class LuceneQueryBuilder
{
    public static string Build(WegFilterCriteria criteria, IReadOnlyList<WegTierDefinition> tiers)
    {
        if (criteria == null)
            throw new ArgumentNullException(nameof(criteria));

        // Collect clauses in a fixed order so the output is deterministic.
        // Deterministic = same criteria -> same query string every time.
        var clauses = new List<string>
        {
            //Required based on the Odin documentation
            "+contentType:WegCard"
        };

        //Domain category filter
        if (!string.IsNullOrWhiteSpace(criteria.DomainVariable))
        {
            clauses.Add($"+categories:{EscapeLuceneTerm(criteria.DomainVariable.Trim())}");
        }

        //Weapon system category fields
        AddOrGroup(clauses, "categories", criteria.WeaponSystemTypeVariable ?? Enumerable.Empty<string>());

        //Origin
        AddOrGroup(clauses, "categories", criteria.ProliferationVariable ?? Enumerable.Empty<string>());

        // Find tier definitions matching the requested tier keys
        if (criteria.TierKey != null && criteria.TierKey.Count > 0)
        {
            var selectedTiers = tiers.Where(t => criteria.TierKey.Any(k => string.Equals(k, t.Key, StringComparison.OrdinalIgnoreCase))).ToList();

            if (selectedTiers.Count == 0)
                //Docs Exceptions: https://learn.microsoft.com/en-us/dotnet/standard/exceptions/
                throw new InvalidOperationException("No matching tiers found.");

            var from = selectedTiers.Min(t => t.From).ToUniversalTime();
            var to = selectedTiers.Max(t => t.To).ToUniversalTime();

            //Docs InvariantCulture: https://learn.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo.invariantculture?view=net-10.0
            var fromStr = from.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            var toStr = to.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);

            clauses.Add($"+WegCard.dateOfIntroduction:[ {fromStr} TO {toStr} ]");
        }

        return string.Join(" ", clauses);
    }

    // Escapes characters that would break a Lucene query.
    // In C#, '\' is an escape character, so "\\" in code produces "\" at runtime.
    // Lucene then needs that "\" to escape special characters like '\' and '"'.
    // This prevents quoted Lucene strings from breaking.
    private static string EscapeLuceneTerm(string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"");
    }

    //Docs IEnumerable: https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1?view=net-10.0
    private static void AddOrGroup(List<string> clauses, string field, IEnumerable<string> values)
    {
        var cleaned = values
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Select(v => $"{field}:{EscapeLuceneTerm(v.Trim())}")
            .ToList();

        if (cleaned.Count == 0) return;

        // +(categories:a categories:b categories:c)
        clauses.Add("+(" + string.Join(" ", cleaned) + ")");
    }
}