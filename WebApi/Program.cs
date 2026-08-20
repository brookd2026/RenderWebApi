using Microsoft.EntityFrameworkCore;
using WebApi.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// If Render passes a postgres:// URL, translate it cleanly
if (!string.IsNullOrEmpty(connectionString) && connectionString.StartsWith("postgres://"))
{
    var databaseUri = new Uri(connectionString);
    var userInfo = databaseUri.UserInfo.Split(':');

    connectionString = $"Host={databaseUri.Host};" +
                       $"Port={databaseUri.Port};" +
                       $"Database={databaseUri.LocalPath.TrimStart('/')};" +
                       $"Username={userInfo[0]};" +
                       $"Password={userInfo[1]};" +
                       $"Pooling=true;" +
                       $"SSL Mode=Require;" +
                       $"Trust Server Certificate=true;";
}

builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// Render assigns a dynamic port via the PORT environment variable
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://*:{port}");

app.MapGet("/", () => "Hello from C# on Render!");

app.Run();