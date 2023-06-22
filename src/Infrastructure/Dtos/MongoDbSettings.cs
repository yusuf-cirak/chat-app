namespace Infrastructure.Dtos;

public sealed class MongoDbSettings
{
    public string ConnectionUri { get; set; }
    public string DatabaseName { get; set; }

    public MongoDbSettings()
    {
        
    }

    public MongoDbSettings(string connectionUri, string databaseName)
    {
        ConnectionUri = connectionUri;
        DatabaseName = databaseName;
    }
}