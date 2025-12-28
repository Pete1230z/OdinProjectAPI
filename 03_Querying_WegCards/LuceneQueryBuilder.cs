namespace OdinProjectAPI.WegSubnav;

//Builds a Lucene query from user selected criteria
//Docs on Static Classes: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/static-classes-and-static-class-members
public static class LuceneQueryBuilder
{
    public static string Build(WegFilterCriteria criteria)
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
            clauses.Add($"+categories: {EscapeLuceneTerm(criteria.DomainVariable.Trim())}");
        }

        //Weapon system category fields
        if (!string.IsNullOrWhiteSpace(criteria.WeaponSystemTypeVariable))
        {
            clauses.Add($"+categories: {EscapeLuceneTerm(criteria.WeaponSystemTypeVariable.Trim())}");
        }

        //Origin
        if (!string.IsNullOrWhiteSpace(criteria.Origin))
        {
            clauses.Add($"+categories: {EscapeLuceneTerm(criteria.Origin.Trim())}");
        }

        //Tier -> dateOfIntroduction range
        // if (criteria.IntroYearFrom.HasValue || criteria.IntroYearTo.HasValue) { ... }

        //If nothing selected, return ":" to match everything.
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
}