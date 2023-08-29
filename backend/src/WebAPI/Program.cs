using System.IO.Compression;
using Application;
using HealthChecks.UI.Client;
using Infrastructure;
using Infrastructure.Constants;
using Infrastructure.SignalR.Hubs;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices();
var mongoDbConnStr = builder.Configuration.GetSection("MongoDbSettings:ConnectionURI").Get<string>();
builder.Services.AddHealthChecks().AddMongoDb(mongoDbConnStr!,name:"MongoDB");


builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

var app = builder.Build();

app.UseResponseCompression();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseHealthChecks("/_health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    
}
); // RequireAuthorization, RequireCors, RequireHost possible for limiting calls for /_health endpoint.

app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>(SignalRConstant.ChatHubUrl);

app.Run();