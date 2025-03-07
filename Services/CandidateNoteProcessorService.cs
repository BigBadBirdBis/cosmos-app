using CosmosApp.Models.Notes;
using Microsoft.Azure.Cosmos;

namespace CosmosApp.Services;
// Create a new service to process the change feed of the CandidateNotes container
// This service will listen for changes in the CandidateNotes container and archive the changes in the CandidateNoteArchives container
public class CandidateNoteProcessorService : BackgroundService
{
    private readonly Container _candidateNotesContainer;
    private readonly Container _candidateNoteArchivesContainer;
    private readonly Container _leasesContainer;

    private ChangeFeedProcessor? _changeFeedProcessor;

    public CandidateNoteProcessorService(IConfiguration configuration, CosmosClient client)
    {
        Database database = client.GetDatabase(configuration["CosmosDb:DatabaseId"] ?? throw new ArgumentNullException("DatabaseId is required."));
        _candidateNotesContainer = database.GetContainer(configuration["CosmosDb:CandidateNotesContainerId"] ?? throw new ArgumentNullException("CandidateNotesContainerId is required."));
        _candidateNoteArchivesContainer = database.GetContainer(configuration["CosmosDb:CandidateNoteArchivesContainerId"] ?? throw new ArgumentNullException("CandidateNoteArchivesContainerId is required."));
        // Used for keeping state of the Change Feed Processor & coordination between multiple instances
        _leasesContainer = database.GetContainer(configuration["CosmosDb:LeasesContainerId"] ?? throw new ArgumentNullException("LeasesContainerId is required."));
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _changeFeedProcessor = await StartChangeFeedProcessorAsync(_candidateNotesContainer, _leasesContainer);
            Console.WriteLine("Change Feed Processor started.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting Change Feed Processor: {ex.Message}");
        }
    }

    private async Task<ChangeFeedProcessor> StartChangeFeedProcessorAsync(Container monitoredContainer, Container leasesContainer)
    {
        ChangeFeedProcessor changeFeedProcessor = monitoredContainer
            .GetChangeFeedProcessorBuilder<CandidateNote>("CandidateNoteVersioningProcessor", HandleChangesAsync)
            .WithInstanceName("CandidateNoteVersioningProcessorComputeInstance")
            .WithLeaseContainer(leasesContainer)
            .Build();

        await changeFeedProcessor.StartAsync();

        return changeFeedProcessor;
    }

    //Implement the HandleChangesAsync method
    private async Task HandleChangesAsync(IReadOnlyCollection<CandidateNote> changes, CancellationToken cancellationToken)
    {
        foreach (CandidateNote candidateNote in changes)
        {
            Console.WriteLine($"Change detected: {candidateNote.Id} - {candidateNote.Title}");
            // new id for the historical collection to preserve the history rather than overwrite it
            CandidateNoteArchive candidateNoteArchive = new(candidateNote);

            // Archive the document
            // Giving the partion key value to the CreateItemAsync method makes the process more efficient
            // as the SDK doesn't have to determine the partition key value
            await _candidateNoteArchivesContainer.CreateItemAsync<CandidateNoteArchive>(candidateNoteArchive, candidateNoteArchive.GetPartitionKey(), cancellationToken: cancellationToken);
        }
    }
}
