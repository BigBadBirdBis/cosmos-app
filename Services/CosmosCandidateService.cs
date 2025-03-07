using CosmosApp.Models;
using Microsoft.Azure.Cosmos;

namespace CosmosApp.Services;
public class CosmosCandidateService
{
    private readonly string _candidatesContainerId;
    private readonly string _candidatesSql;
    private Database _database;


    public CosmosCandidateService(IConfiguration configuration, CosmosClient client)
    {
        _database = client.GetDatabase(configuration["CosmosDb:DatabaseId"] ?? throw new ArgumentNullException("DatabaseId is required."));
        _candidatesContainerId = configuration["CosmosDb:CandidatesContainerId"] ?? throw new ArgumentNullException("CandidatesContainerId is required.");

        _candidatesSql = @"
SELECT
    *
FROM " + _candidatesContainerId + @"
c
WHERE c.id = @id
";
    }



    public async Task CreateCandidateAsync(Candidate candidate)
    {
        Container container = _database.GetContainer(_candidatesContainerId);
        try
        {
            // Giving the partion key value to the CreateItemAsync method makes the process more efficient
            // as the SDK doesn't have to determine the partition key value;
            ItemResponse<Candidate> response = await container.CreateItemAsync(candidate, candidate.GetPartitionKey());
            //Console.WriteLine($"[{response.StatusCode}]\t{response.ActivityId}\t{response.RequestCharge} RUs");
        }
        catch(CosmosException ex)
        {
            Console.WriteLine($"[{ex.StatusCode}]\t{ex.ActivityId}\t{ex.RequestCharge} RUs");
        }
    }

    public async Task<Candidate> GetCandidateAsync(string id)
    {
        Container container = _database.GetContainer(_candidatesContainerId);

        //ItemResponse<Candidate> response = await container.ReadItemAsync<Candidate>(id, new PartitionKey(id));

        QueryDefinition _queryDefinition = new QueryDefinition(_candidatesSql).WithParameter("@id", id);
        using FeedIterator<Candidate> feedIterator = container.GetItemQueryIterator<Candidate>(_queryDefinition);
        FeedResponse<Candidate> response = await feedIterator.ReadNextAsync();

        return response.Resource.First();
    }
}