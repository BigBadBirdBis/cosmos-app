using CosmosApp.Models.Notes;
using Microsoft.Azure.Cosmos;

namespace CosmosApp.Services;
public class CosmosCandidateNotesService
{
    private readonly string _candidateNotesContainerId;
    private readonly string _candidateNotesSql;
    private Database _database;


    public CosmosCandidateNotesService(IConfiguration configuration, CosmosClient client)
    {
        _database = client.GetDatabase(configuration["CosmosDb:DatabaseId"] ?? throw new ArgumentNullException("DatabaseId is required."));
        _candidateNotesContainerId = configuration["CosmosDb:CandidateNotesContainerId"] ?? throw new ArgumentNullException("CandidatesNotesContainerId is required.");

        _candidateNotesSql = @"
SELECT
    *
FROM " + _candidateNotesContainerId + @"
c
WHERE c.id = @id
";
    }



    public async Task CreateCandidateNoteAsync(CandidateNote note)
    {
        Container container = _database.GetContainer(_candidateNotesContainerId);
        try
        {
            // Giving the partion key value to the CreateItemAsync method makes the process more efficient
            // as the SDK doesn't have to determine the partition key value
            ItemResponse<CandidateNote> response = await container.CreateItemAsync(note, note.GetPartitionKey());
            //Console.WriteLine($"[{response.StatusCode}]\t{response.ActivityId}\t{response.RequestCharge} RUs");
        }
        catch(CosmosException ex)
        {
            Console.WriteLine($"[{ex.StatusCode}]\t{ex.ActivityId}\t{ex.RequestCharge} RUs");
        }
    }

    public async Task<CandidateNote?> GetCandidateNoteAsync(string id)
    {
        CandidateNote? result = null;
        Container container = _database.GetContainer(_candidateNotesContainerId);

        QueryDefinition _queryDefinition = new QueryDefinition(_candidateNotesSql).WithParameter("@id", id);
        using FeedIterator<CandidateNote> feedIterator = container.GetItemQueryIterator<CandidateNote>(_queryDefinition);
        FeedResponse<CandidateNote> response = await feedIterator.ReadNextAsync();

        if (response.Count > 0)
        {
            result = response.Resource.First();
        }

        return result;
    }

    
    public async Task UpdateCandidateNoteContentAsync(string id, string content)
    {
        Container container = _database.GetContainer(_candidateNotesContainerId);
        try
        {
            CandidateNote note = await GetCandidateNoteAsync(id) ?? throw new Exception("Note not found.");
            note.Content = content;
            note.DocumentVersion++;

            ItemResponse<CandidateNote> response = await container.UpsertItemAsync(note);
            //Console.WriteLine($"[{response.StatusCode}]\t{response.ActivityId}\t{response.RequestCharge} RUs");
        }
        catch(CosmosException ex)
        {
            Console.WriteLine($"[{ex.StatusCode}]\t{ex.ActivityId}\t{ex.RequestCharge} RUs");
            throw;
        }
    }
}