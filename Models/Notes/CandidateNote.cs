using Microsoft.Azure.Cosmos;

namespace CosmosApp.Models.Notes;
public class CandidateNote : AbstractNote
{
    public string CandidateId { get; set; }
    public int DocumentVersion { get; set; }

    public static readonly List<string> s_CosmosContainerKeyPaths =
        [
            "/CandidateId",
            "/id"
        ];

    public override PartitionKey GetPartitionKey() => new PartitionKeyBuilder().Add(CandidateId).Add(Id).Build();


    public CandidateNote(string id, string title, string content, string candidateId, int documentVersion = 0)
    {
        Id = id;
        Title = title;
        Content = content;
        CandidateId = candidateId;
        DocumentVersion = documentVersion;
    }

    public CandidateNote()
    {
        Id = "0"; //Guid.NewGuid().ToString();
        Title = "Default Title";
        Content = "Default Content";
        CandidateId = "0";
        DocumentVersion = 0;
    }
}