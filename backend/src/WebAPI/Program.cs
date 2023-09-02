using Application;
using HealthChecks.UI.Client;
using Infrastructure;
using Infrastructure.SignalR.Hubs;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using WebAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddResponseCompressionServices(); // From WebAPI\Extensions\ResponseCompressionExtensions.cs

builder.Services.AddHealthCheckServices(builder.Configuration); // From WebAPI\Extensions\HealthCheckExtensions.cs

builder.Services.AddApplicationServices(); // From Application\ServiceRegistration.cs

builder.Services.AddInfrastructureServices(builder.Configuration); // From Infrastructure\ServiceRegistration.cs

builder.Services.AddJwtAuthenticationServices(builder.Configuration); // From WebAPI\Extensions\JwtBearerExtensions.cs

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGenServices(); // From WebAPI\Extensions\SwaggerExtensions.cs

builder.Services.AddSignalR();

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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/_chat");

app.Run();