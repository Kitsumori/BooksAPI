using Microsoft.EntityFrameworkCore;
using BooksApi.DTO;
namespace BooksApi.DB;
public class BooksDbContext : DbContext
{
    public BooksDbContext(DbContextOptions<BooksDbContext> options) : base(options) {}

    public DbSet<BookDTO> Books { get; set; }
}