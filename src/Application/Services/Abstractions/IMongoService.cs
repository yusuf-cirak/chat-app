using Domain.Common;
using MongoDB.Driver;

namespace Application.Services.Abstractions;

public interface IMongoService<T>
    where T : BaseEntity, new()
{
    IMongoCollection<T> Collection { get; }
}