using TaskManagementApi.Core;
using TaskManagementApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCoreServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddControllers(); // This enables MVC controllers
builder.Services.AddEndpointsApiExplorer(); // Required for Swagger to discover endpoints
builder.Services.AddSwaggerGen(); // Configures Swagger generation

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Serves the Swagger JSON document
    app.UseSwaggerUI(); // Serves the Swagger UI (HTML, JS, CSS)
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers(); // This maps the routes for your MVC controllers

app.Run();