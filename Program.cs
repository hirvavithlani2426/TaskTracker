using Microsoft.EntityFrameworkCore;
using TaskTracker.Api.Data;
using TaskTracker.Api.Services;
using Swashbuckle.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Automatically serializes and deserializes enums as readable strings
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ENVIRONMENT SENSITIVE CONTEXT ROUTING
if (builder.Environment.IsDevelopment())
{
    var localConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=tasks.db";
    builder.Services.AddDbContext<TaskDbContext>(options =>
        options.UseSqlite(localConnectionString));
}
else
{
    var cloudConnectionString = builder.Configuration.GetConnectionString("AzureSqlConnection");
    builder.Services.AddDbContext<TaskDbContext>(options =>
        options.UseSqlServer(cloudConnectionString, sqlServerOptions =>
        {
            // Tells EF Core to retry if Azure SQL is temporarily busy waking up
            sqlServerOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        }));
}

builder.Services.AddScoped<ITaskService, TaskService>();

var app = builder.Build();

// AUTOMATIC DB INTIALIZATION
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
    // Auto-generates the local database files or remote tables if missing
    dbContext.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();