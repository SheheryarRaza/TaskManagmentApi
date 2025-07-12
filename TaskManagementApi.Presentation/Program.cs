using TaskManagementApi.Core;
using TaskManagementApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCoreServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddControllers(); // This enables MVC controllers
builder.Services.AddEndpointsApiExplorer(); // Required for Swagger to discover endpoints
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
}); // Configures Swagger generation

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