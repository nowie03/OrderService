using Microsoft.EntityFrameworkCore;
using orderService.Models;
using OrderService.Models;

namespace orderService.Context
{
    public class ServiceContext : DbContext
    {
        public ServiceContext(DbContextOptions options) : base(options) { }

        public DbSet<Order> Orders { get; set; }


        public DbSet<Message> Outbox { get; set; }
    }
}
