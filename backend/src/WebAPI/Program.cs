using Application;
using HealthChecks.UI.Client;
using Infrastructure;
using Infrastructure.SignalR;
using Infrastructure.SignalR.Hubs;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

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

var app = builder.Build();

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

app.MapHub<ChatHub>(SignalRConstants.ChatHubUrl);

app.Run();