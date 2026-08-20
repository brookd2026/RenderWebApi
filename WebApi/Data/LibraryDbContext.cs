using Microsoft.EntityFrameworkCore;

namespace WebApi.Data
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
        {
        }

        // Add your database tables (DbSets) here, for example:
        // public DbSet<Book> Books { get; set; }
    }
}
