using CosmosApp.Models;
using CosmosApp.Models.Notes;
using Microsoft.Azure.Cosmos;

namespace CosmosApp.Services;
public class CosmosSetupService
{
    private readonly CosmosClient _client;
    // Store Candidates
    private readonly string _candidatesContainerId;
    // Store Notes
    private readonly string _candidateNotesContainerId;
    // Store Notes History
    private readonly string _candidateNoteArchivesContainerId;
    // Used to store leases for the Change Feed Processor
    private readonly string _leasesContainerId;
    private Database _database;


    public CosmosSetupService(IConfiguration configuration, CosmosClient client)
    {
        _database = client.GetDatabase(configuration["CosmosDb:DatabaseId"] ?? throw new ArgumentNullException("DatabaseId is required."));
        _candidatesContainerId = configuration["CosmosDb:CandidatesContainerId"] ?? throw new ArgumentNullException("CandidatesContainerId is required.");
        _candidateNotesContainerId = configuration["CosmosDb:CandidateNotesContainerId"] ?? throw new ArgumentNullException("CandidateNotesContainerId is required.");
        _candidateNoteArchivesContainerId = configuration["CosmosDb:CandidateNoteArchivesContainerId"] ?? throw new ArgumentNullException("CandidateNoteArchivesContainerId is required.");
        _leasesContainerId = configuration["CosmosDb:LeasesContainerId"] ?? throw new ArgumentNullException("LeasesContainerId is required.");

        _client = client;
    }
    


    private async Task CreateContainerAsync(string containerId, List<string> partitionKeyPaths)
    {
        ContainerProperties containerProperties = new(
            id: containerId,
            partitionKeyPaths: partitionKeyPaths
        );

        await _database.CreateContainerIfNotExistsAsync(containerProperties);
        Console.WriteLine($"Container {containerId} created successfully.");
    }

    public async Task CreateAllContainersAsync()
    {
        _database = await _client.CreateDatabaseIfNotExistsAsync(_database.Id);

        List<string> leaseContainerKeyPaths =
        [
            "/id"
        ];

        await CreateContainerAsync(_candidatesContainerId, Candidate.s_CosmosContainerKeyPaths);
        await CreateContainerAsync(_candidateNotesContainerId, CandidateNote.s_CosmosContainerKeyPaths);
        await CreateContainerAsync(_candidateNoteArchivesContainerId, CandidateNoteArchive.s_CosmosContainerKeyPaths);
        await CreateContainerAsync(_leasesContainerId, leaseContainerKeyPaths);

        Console.WriteLine("All containers created successfully.");
    }

    private async Task DeleteContainerAsync(string containerId)
    {
        Container container = _database.GetContainer(containerId);
        try
        {
            await container.DeleteContainerAsync();
            Console.WriteLine($"Container {containerId} deleted successfully.");
        }
        catch (CosmosException ex)
        {
            Console.WriteLine($"Container {containerId} failed to delete. {ex.Message}");
            return;
        }
    }

    public async Task DeleteAllContainersAsync()
    {
        await DeleteContainerAsync(_candidatesContainerId);
        await DeleteContainerAsync(_candidateNotesContainerId);
        await DeleteContainerAsync(_candidateNoteArchivesContainerId);
        await DeleteContainerAsync(_leasesContainerId);
    }
}