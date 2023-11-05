using Application.Abstractions.Helpers;
using Application.Abstractions.Services;
using Application.Abstractions.Services.Chat;
using Application.Abstractions.Services.Image;
using CloudinaryDotNet;
using Domain.Entities;
using ElasticSearch;
using Infrastructure.Dtos;
using Infrastructure.Helpers.Hashing;
using Infrastructure.Helpers.JWT;
using Infrastructure.Persistence;
using Infrastructure.Services.Chat;
using Infrastructure.Services.Image;
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

        services.AddSingleton<IChatService, InMemoryChatService>();

        services.AddSingleton<Cloudinary>(_ =>
            new Cloudinary(account: configuration.GetSection("CloudinarySettings").Get<Account>()));

        services.AddScoped<IImageService, CloudinaryImageService>();

        services.AddSignalR();

        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));

        services.AddScoped<IMongoService, MongoService>();

        services.AddSingleton<IElasticSearchManager, ElasticSearchManager>();

        // Generate seed data if less than 1000 message exist in database.
        // var mongoService = services.BuildServiceProvider().GetRequiredService<IMongoService>();
        // if (mongoService.GetCollection<Message>().CountDocuments(_ => true) < 500)
        // {
        //     new SeedDataGenerator().GenerateAndPersist(mongoService);
        // }

        var elasticSearchManager = services.BuildServiceProvider().GetRequiredService<IElasticSearchManager>();

        // Create users index for elastic search. Its okay if it already exists.
        var createUserIndexResult = elasticSearchManager.CreateNewIndexAsync(indexModel =>
        {
            indexModel.IndexName = "users";
            indexModel.AliasName = "search";
            indexModel.NumberOfReplicas = 1;
            indexModel.NumberOfShards = 1;
        }).GetAwaiter().GetResult();
    }
}