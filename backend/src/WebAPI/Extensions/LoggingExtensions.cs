using ElasticSearch.Config;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;

namespace WebAPI.Extensions;

public static class LoggingExtensions
{
    public static void ConfigureLogging(this WebApplicationBuilder builder)
    {
        var elasticConfig = builder.Configuration.GetSection("ElasticSearchConfig").Get<ElasticSearchConfig>() ??
                            throw new Exception("ElasticSearchConfig is null");

        var logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Async(log => log.Elasticsearch(
                new ElasticsearchSinkOptions(new Uri(elasticConfig.ConnectionString))
                {
                    IndexFormat = $"log-{DateTime.UtcNow:yyyy-MM}",
                    AutoRegisterTemplate = true,
                    MinimumLogEventLevel = LogEventLevel.Information,
                    CustomFormatter = new ExceptionAsObjectJsonFormatter(renderMessage: true),
                    NumberOfShards = 1,
                    NumberOfReplicas = 1,
                    ModifyConnectionSettings = connectionConfiguration => connectionConfiguration
                        .BasicAuthentication(elasticConfig.UserName, elasticConfig.Password)
                }))
            .CreateLogger();

        builder.Host.UseSerilog(logger);
    }

    public static void UseLogging(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
    }
}