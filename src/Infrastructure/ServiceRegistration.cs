using Infrastructure.Dtos;
using Infrastructure.JWT;
using Infrastructure.Persistence.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ServiceRegistration
{
    public static void AddInfrastructureServices(this IServiceCollection services,IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));
        
        services.AddScoped(typeof(IMongoCollectionService<>), typeof(MongoCollectionService<>));

        services.AddSingleton<IJwtHelper, JwtHelper>();

    }
}