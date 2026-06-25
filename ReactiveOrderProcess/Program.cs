using Microsoft.EntityFrameworkCore;
using ReactiveOrderProcess.Core.Interfaces;
using ReactiveOrderProcess.Core.Services;
using ReactiveOrderProcess.Infrastructure.Data;
using ReactiveOrderProcess.Infrastructure.Data.Repositories;
using ReactiveOrderProcess.Infrastructure.Messaging;
using ReactiveOrderProcess.Workers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Database Configuration

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source = orders.db";
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Application Services    
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// Register InMemory channel for pub/sub
builder.Services.AddSingleton<InMemoryChannel>();
builder.Services.AddSingleton<IMessagePublisher>(provider => provider.GetRequiredService<InMemoryChannel>());

// Register the background worker
builder.Services.AddHostedService<OrderProcessingWorker>();

var app = builder.Build();

// Database Auto-Migration / Initialization Routine on Startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        logger.LogInformation("Applying automatic database migrations...");
        var context = services.GetRequiredService<OrderDbContext>();
        await context.Database.MigrateAsync();
        logger.LogInformation("Database migrated and initialized successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while automatically migrating/creating the SQLite database.");
    }
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
