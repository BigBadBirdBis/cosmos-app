using Microsoft.Azure.Cosmos;

namespace CosmosApp.Models;
public class Candidate : AbstractSchemaObject
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? LinkedIn { get; set; }
    public string? GitHub { get; set; }
    public string? PortfolioId { get; set; }
    public string? ResumeId { get; set; }
    public string? NotesId { get; set; }

    public static readonly List<string> s_CosmosContainerKeyPaths =
    [
        "/id",
        "/Name"
    ];

    public override PartitionKey GetPartitionKey() => new PartitionKeyBuilder().Add(Id).Add(Name).Build();

    public Candidate(string id, string name, string email, string phone, string linkedIn, string gitHub, string portfolioId, string resumeId, string notesId)
    {
        Id = id;
        Name = name;
        Email = email;
        Phone = phone;
        LinkedIn = linkedIn;
        GitHub = gitHub;
        PortfolioId = portfolioId;
        ResumeId = resumeId;
        NotesId = notesId;
    }

    public Candidate()
    {
        Id = "0"; //Guid.NewGuid().ToString();
        Name = "Default Name";
        Email = "Default Email";
        Phone = "Default Phone";
        LinkedIn = "Default LinkedIn";
        GitHub = "Default GitHub";
        PortfolioId = "0";
        ResumeId = "0";
        NotesId = "0";
    }
}