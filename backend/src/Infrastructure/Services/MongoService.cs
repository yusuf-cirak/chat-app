using Application.Abstractions.Services;
using Domain.Common;
using Infrastructure.Dtos;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Infrastructure.Services;

public sealed class MongoService<T> : IMongoService<T>
    where T : BaseEntity, new()
{
    public MongoService(IOptions<MongoDbSettings> mongoDbSettings)
    {
        MongoClient client = new(mongoDbSettings.Value.ConnectionUri);
        IMongoDatabase db = client.GetDatabase(mongoDbSettings.Value.DatabaseName);
        Collection = db.GetCollection<T>(typeof(T).Name);
    }

    public IMongoCollection<T> Collection { get; }
}