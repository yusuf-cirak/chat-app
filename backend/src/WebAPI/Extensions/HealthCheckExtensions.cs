namespace WebAPI.Extensions;

internal static class HealthCheckExtensions
{
    internal static void AddHealthCheckServices(this IServiceCollection services,IConfiguration configuration)
    {
        var mongoDbConnStr = configuration.GetSection("MongoDbSettings:ConnectionURI").Get<string>();
        services.AddHealthChecks().AddMongoDb(mongoDbConnStr!,name:"MongoDB");
    }
}