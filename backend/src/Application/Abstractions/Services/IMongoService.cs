using Domain.Common;
using MongoDB.Driver;

namespace Application.Abstractions.Services;

public interface IMongoService<T>
    where T : BaseEntity, new()
{
    IMongoCollection<T> Collection { get; }
}