using Domain.Common;
using MongoDB.Driver;

namespace Application.Abstractions.Services;

public interface IMongoService
{
    public IMongoCollection<T> GetCollection<T>();
}