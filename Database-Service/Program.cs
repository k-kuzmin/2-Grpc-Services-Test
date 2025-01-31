using Domain;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddScoped<DbMigrationService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

var app = builder.Build();

ApplyMigrate(app);

// Configure the HTTP request pipeline.
app.MapGrpcService<CurrencyGrpcService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();

static void ApplyMigrate(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var migrationService = scope.ServiceProvider.GetRequiredService<DbMigrationService>();
        migrationService.Migrate();
    }
}