using ContadorTabaco.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ContadorTabaco.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly()); //aplica as confs dos modelos criados em Configurations
        base.OnModelCreating(modelBuilder);
    }
}
