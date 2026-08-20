var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Render assigns a dynamic port via the PORT environment variable
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://*:{port}");

app.MapGet("/", () => "Hello from C# on Render!");

app.Run();