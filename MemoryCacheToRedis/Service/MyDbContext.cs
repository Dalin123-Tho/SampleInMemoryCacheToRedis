using MemoryCacheToRedis.Models;
using Microsoft.EntityFrameworkCore;

namespace MemoryCacheToRedis.Service
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options) { }

        public DbSet<Employee> Employees { get; set; }

    }   
}
