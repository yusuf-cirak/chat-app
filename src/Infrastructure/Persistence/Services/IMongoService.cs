using Domain.Common;
using MongoDB.Driver;

namespace Infrastructure.Persistence.Services;

public interface IMongoService<T>
    where T : BaseEntity, new()
{
     IMongoCollection<T> Collection { get; set; }
}