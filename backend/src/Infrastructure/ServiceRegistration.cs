using Application.Abstractions.Helpers;
using Application.Abstractions.Services;
using Application.Abstractions.Services.Chat;
using Infrastructure.Dtos;
using Infrastructure.Helpers.Hashing;
using Infrastructure.Helpers.JWT;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Infrastructure.Services.Chat;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ServiceRegistration
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));

        services.AddScoped<IMongoService, MongoService>();

        services.AddSingleton<IJwtHelper, JwtHelper>();
        services.AddSingleton<IHashingHelper, HashingHelper>();

        services.AddSingleton<IChatService,InMemoryChatService>();
        services.AddSingleton<IChat, Chat>();
        
        services.AddSignalR();
    }
}