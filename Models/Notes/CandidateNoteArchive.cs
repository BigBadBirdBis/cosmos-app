using Microsoft.Azure.Cosmos;

namespace CosmosApp.Models.Notes
{
    public class CandidateNoteArchive : CandidateNote
    {
        public string DocumentId { get; set; }

        public static new readonly List<string> s_CosmosContainerKeyPaths =
        [
            "/DocumentId",
            "/id"
        ];

        public override PartitionKey GetPartitionKey() => new PartitionKeyBuilder().Add(DocumentId).Add(Id).Build();

        public CandidateNoteArchive(CandidateNote candidateNotes)
            : base(candidateNotes.Id, candidateNotes.Title, candidateNotes.Content, candidateNotes.CandidateId, candidateNotes.DocumentVersion)
        {
            Id = Guid.NewGuid().ToString();
            DocumentId = candidateNotes.Id;
        }

        public CandidateNoteArchive()
            : base()
        {
            DocumentVersion = 0;
        }
    }
}