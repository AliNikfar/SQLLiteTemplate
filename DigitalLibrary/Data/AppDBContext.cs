using DigitalLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace DigitalLibrary.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options)
            :base(options) 
        {

        }
        public DbSet<Book> Books { get; set; }
    }
}
