using Microsoft.EntityFrameworkCore;
using orderService.Models;

namespace orderService.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Order> Orders { get; set; }
    }
}
