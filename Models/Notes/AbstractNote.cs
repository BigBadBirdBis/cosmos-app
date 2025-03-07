namespace CosmosApp.Models.Notes;
public abstract class AbstractNote : AbstractSchemaObject
{
    public string Title { get; set; }
    public string Content { get; set; }

    public AbstractNote(string id, string title, string content)
    {
        Id = id;
        Title = title;
        Content = content;
    }

    public AbstractNote()
    {
        Id = Guid.NewGuid().ToString();
        Title = "Default Title";
        Content = "Default Content";
    }
}