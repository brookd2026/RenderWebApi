using Microsoft.EntityFrameworkCore;

namespace WebApi.Data
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
        {
        }

        // This line tells Entity Framework to create a "Books" table
        public DbSet<Book> Books { get; set; }
    }
}
