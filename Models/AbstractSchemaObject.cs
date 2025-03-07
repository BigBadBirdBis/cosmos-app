using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

namespace CosmosApp.Models;
public abstract class AbstractSchemaObject
{
    // if no id in lowercase, cosmos' create item throws exception
    [JsonProperty("id")]
    public string Id { get; set; }

    // If you change the version, explain why in schema.md
    public readonly Version SchemaVersion = new(0, 0, 1);

    public abstract PartitionKey GetPartitionKey();
}