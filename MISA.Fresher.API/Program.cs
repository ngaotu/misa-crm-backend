using Dapper;
using MISA.CRM.Core.Interfaces.Repositories;
using MISA.CRM.Infrastructure.Repositories;
using MISA.CRM.Core.Interfaces.Services;
using MISA.CRM.Core.Services;
using MISA.CRM.API.Middlewares;
using MISA.CRM.Infrastructure.Seeders;
using MISA.CRM.Core.Exceptions;
using MISA.CRM.Infrastructure.Services;
using MISA.CRM.Core.Options;

var builder = WebApplication.CreateBuilder(args);

// Enable mapping customer_id -> CustomerId, customer_name -> CustomerName, ...
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

// Add services to the container.
builder.Services.AddControllers();

// Configure FileStorageOptions from configuration
builder.Services.Configure<FileStorageOptions>(builder.Configuration.GetSection("FileStorage"));

// If WebRootPath is available, ensure default RootPath is set to WebRootPath when option not provided
builder.Services.PostConfigure<FileStorageOptions>(opts =>
{
    var webRoot = builder.Environment.WebRootPath;
    if (string.IsNullOrWhiteSpace(opts.RootPath) && !string.IsNullOrWhiteSpace(webRoot))
    {
        opts.RootPath = webRoot;
    }
});

// Configure CORS for Vue.js frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("VueJsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Register application services and repositories for DI
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

builder.Services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

// File storage - register implementation from Infrastructure project
builder.Services.AddScoped<IFileStorageService, MISA.CRM.Infrastructure.Services.LocalFileStorageService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS - PHẢI đặt trước UseAuthorization
app.UseCors("VueJsPolicy");

// Seed data CSV file when running in Development only
if (app.Environment.IsDevelopment())
{
    try
    {
        var seedPath = Path.Combine(app.Environment.ContentRootPath, "customers_seed3.csv");
        // Generate2000 records by default (adjust as needed)
        CustomerSeeder.GenerateCsv(1000, seedPath);
        Console.WriteLine($"Customer seed CSV generated: {seedPath}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to generate seed CSV: {ex.Message}");
    }

    // If SEED_ONLY env var is set to1, exit after seeding (one-off run)
    var seedOnly = Environment.GetEnvironmentVariable("SEED_ONLY");
    if (!string.IsNullOrEmpty(seedOnly) && seedOnly == "1")
    {
        Console.WriteLine("SEED_ONLY=1 detected, exiting after seeding.");
        return;
    }
}

app.UseHttpsRedirection();

// Serve static files (wwwroot) so avatars are reachable
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
