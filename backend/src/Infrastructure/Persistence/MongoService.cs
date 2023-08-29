using Application.Abstractions.Services;
using Infrastructure.Dtos;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Infrastructure.Persistence;

public sealed class MongoService : IMongoService
{
    private readonly IMongoDatabase _database;
    
    public MongoService(IOptions<MongoDbSettings> mongoDbSettings)
    {
        MongoClient client = new(mongoDbSettings.Value.ConnectionUri);
        _database =  client.GetDatabase(mongoDbSettings.Value.DatabaseName);
    }
    public IMongoCollection<T> GetCollection<T>() => _database.GetCollection<T>(typeof(T).Name);
}