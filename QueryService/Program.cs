using StackExchange.Redis;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Configure Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect("redis:6379")); // Use correct Redis host

builder.Services.AddSingleton<IDatabase>(sp => sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase());

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddRedis("redis:6379", name: "Redis", timeout: TimeSpan.FromSeconds(5));

// Add RabbitMQListener as a hosted service
builder.Services.AddHostedService<RabbitMQListener>();

// Set application to listen on port 80
builder.WebHost.UseUrls("http://*:80");

// Add controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection(); // Disable HTTPS redirection in non-Docker environments
}

app.UseAuthorization();

// Map controllers
app.MapControllers();

// Map health check endpoint
app.MapHealthChecks("/health");

app.Run();