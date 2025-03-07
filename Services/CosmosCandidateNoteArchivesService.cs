using CosmosApp.Models.Notes;
using Microsoft.Azure.Cosmos;

namespace CosmosApp.Services;
public class CosmosCandidateNoteArchivesService
{
    private readonly string _candidatesNoteArchivesContainerId;
    private readonly string _candidatesNoteArchivesSql;
    private Database _database;


    public CosmosCandidateNoteArchivesService(IConfiguration configuration, CosmosClient client)
    {
        _database = client.GetDatabase(configuration["CosmosDb:DatabaseId"] ?? throw new ArgumentNullException("DatabaseId is required."));
        _candidatesNoteArchivesContainerId = configuration["CosmosDb:CandidateNoteArchivesContainerId"] ?? throw new ArgumentNullException("CandidateNoteArchivesContainerId is required.");

        _candidatesNoteArchivesSql = @"
SELECT
    *
FROM " + _candidatesNoteArchivesContainerId + @"
c
WHERE c.id = @id
";
    }


/*
    public static async Task CreateCandidateNoteAsync(VersionedCandidateNote note)
    {
        Container container = _database.GetContainer(candidatesNotesArchiveContainerId);
        try
        {
            ItemResponse<VersionedCandidateNote> response = await container.CreateItemAsync(note);
            Console.WriteLine($"[{response.StatusCode}]\t{response.ActivityId}\t{response.RequestCharge} RUs");
        }
        catch(CosmosException ex)
        {
            Console.WriteLine($"[{ex.StatusCode}]\t{ex.ActivityId}\t{ex.RequestCharge} RUs");
        }
    }
    */

    public async Task<CandidateNoteArchive> GetCandidateNoteArchiveAsync(string id)
    {
        Container container = _database.GetContainer(_candidatesNoteArchivesContainerId);

        QueryDefinition _queryDefinition = new QueryDefinition(_candidatesNoteArchivesSql).WithParameter("@id", id);
        using FeedIterator<CandidateNoteArchive> feedIterator = container.GetItemQueryIterator<CandidateNoteArchive>(_queryDefinition);
        FeedResponse<CandidateNoteArchive> response = await feedIterator.ReadNextAsync();

        return response.Resource.First();
    }
}