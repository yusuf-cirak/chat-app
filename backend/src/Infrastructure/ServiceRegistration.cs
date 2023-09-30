using Application.Abstractions.Helpers;
using Application.Abstractions.Services;
using Application.Abstractions.Services.Chat;
using Domain.Entities;
using Infrastructure.Dtos;
using Infrastructure.Helpers.Hashing;
using Infrastructure.Helpers.JWT;
using Infrastructure.Persistence;
using Infrastructure.Services.Chat;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Infrastructure;

public static class ServiceRegistration
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IJwtHelper, JwtHelper>();
        services.AddSingleton<IHashingHelper, HashingHelper>();

        services.AddSingleton<IChatService,InMemoryChatService>();
        services.AddSingleton<IChat, Chat>();
        
        services.AddSignalR();
        
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));

        services.AddScoped<IMongoService, MongoService>();

        // Generate seed data if less than 1000 message exist in database.
        var mongoService = services.BuildServiceProvider().GetRequiredService<IMongoService>();
        if (mongoService.GetCollection<Message>().CountDocuments(_ => true) < 500)
        {
            new SeedDataGenerator().GenerateAndPersist(mongoService);
        }
    }
}