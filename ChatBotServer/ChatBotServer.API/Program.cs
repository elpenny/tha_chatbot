using ChatBotServer.Infrastructure;
using ChatBotServer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "ChatBot API",
        Version = "v1",
        Description = "API for AI ChatBot with streaming support, conversation management, and message rating",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "ChatBot API",
            Email = "support@chatbot.com"
        }
    });
    
    // Include XML comments
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = System.IO.Path.Combine(System.AppContext.BaseDirectory, xmlFile);
    if (System.IO.File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
    
    // Ensure all controllers are documented
    c.DocInclusionPredicate((name, api) => true);
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ChatBotServer.Application.DependencyInjection).Assembly));
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Auto-migrate database on startup
try
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}
catch (Exception ex)
{
    Console.WriteLine($"Database migration failed: {ex.Message}");
    throw;
}

// Configure the HTTP request pipeline.
// Always enable Swagger for containerized environments
app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

// Skip HTTPS redirection in containerized environment
if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

// Enable CORS
app.UseCors("AllowAngularDev");

app.MapControllers();

// Add a simple health check endpoint
app.MapGet("/health", () => new { 
    status = "healthy", 
    timestamp = DateTime.UtcNow,
    environment = app.Environment.EnvironmentName
});


app.Run();