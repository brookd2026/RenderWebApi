using Microsoft.AspNetCore.Mvc;
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

// Add these two lines to register Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ==========================================
// 2. CONFIGURE MIDDLEWARE PIPELINE
// ==========================================

// Add these lines to enable Swagger UI on your live URL
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
    c.RoutePrefix = "swagger"; // Exposes UI at ://onrender.com
});

// Render assigns a dynamic port via the PORT environment variable
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://*:{port}");

// ==========================================
// 3. DEFINE API ENDPOINTS
// ==========================================


app.MapGet("/", () => "Hello from C# on Render!");

// 1. GET: Read all books from the database
app.MapGet("/books", async (LibraryDbContext db) =>
    await db.Books.ToListAsync());

// 2. POST: Add a new book to the database (No Swagger needed if using Method 2 below)
app.MapPost("/books", async ([FromBody] Book book, LibraryDbContext db) =>
{
    db.Books.Add(book);
    await db.SaveChangesAsync();
    return Results.Created($"/books/{book.Id}", book);
});

// 3. PUT: Update an existing book's details
app.MapPut("/books/{id}", async (int id, Book updatedBook, LibraryDbContext db) =>
{
    var book = await db.Books.FindAsync(id);
    if (book is null) return Results.NotFound();

    book.Title = updatedBook.Title;
    book.Author = updatedBook.Author;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// 4. DELETE: Remove a book from the database
app.MapDelete("/books/{id}", async (int id, LibraryDbContext db) =>
{
    if (await db.Books.FindAsync(id) is Book book)
    {
        db.Books.Remove(book);
        await db.SaveChangesAsync();
        return Results.Ok(book);
    }
    return Results.NotFound();
});
app.Run();