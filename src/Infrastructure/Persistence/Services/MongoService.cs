using Domain.Common;
using Infrastructure.Dtos;
using MongoDB.Driver;
using Microsoft.Extensions.Options;

namespace Infrastructure.Persistence.Services;

public sealed class MongoService<T> : IMongoService<T>
    where T : BaseEntity, new()
{
    public IMongoCollection<T> Collection { get; set; }

    public MongoService(IOptions<MongoDbSettings> mongoDbSettings)
    {
        MongoClient client = new(mongoDbSettings.Value.ConnectionUri);
        IMongoDatabase db = client.GetDatabase(mongoDbSettings.Value.DatabaseName);
        Collection = db.GetCollection<T>(typeof(T).Name);
    }
}