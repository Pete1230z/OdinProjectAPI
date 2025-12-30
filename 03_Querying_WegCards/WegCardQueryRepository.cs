using OdinProjectAPI.GraphQL;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OdinProjectAPI.WegSubnav;

public sealed class WegCardQueryRepository
{
    private readonly GraphQLTransportClient _client;

    public WegCardQueryRepository(GraphQLTransportClient client) 
    {
        _client = client;
    }

    public async Task<List<WegCardItem>> GetWegCardsAsync(string LuceneQuery, int limit = 700, int offset = 0)
    {
        var gql = @"
        query WegCards($query: String!, $limit: Int!, $offset: Int!) {
          wegCardCollection(query: $query, limit: $limit, offset: $offset) {
              name
              sections
              images
              origin { name velocityVar }
            }
        }";

        var variables = new
        {
            query = LuceneQuery,
            limit,
            offset
        };

        var rawJson = await _client.ExecuteRawAsync(gql, variables);

        var parsed = JsonSerializer.Deserialize<GraphQLResponse<WegCardCollectionData>>(rawJson, new JsonSerializerOptions {  PropertyNameCaseInsensitive = true});

        return parsed?.Data?.WegCardCollection ?? new List<WegCardItem>();
    }
}