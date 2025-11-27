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

builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();

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


app.UseHttpsRedirection();

// Serve static files (wwwroot) so avatars are reachable
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
