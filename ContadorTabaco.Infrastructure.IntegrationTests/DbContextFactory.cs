using ContadorTabaco.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ContadorTabaco.Infrastructure.IntegrationTests;

public static class DbContextFactory
{
    public static AppDbContext Create()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionString,                
                // Dizemos explicitamente ao EF Core para procurar as migrações
                // no assembly do projeto "ContadorTabaco.Infrastructure".
                b => b.MigrationsAssembly("ContadorTabaco.Infrastructure")
            )
            .Options;

        var context = new AppDbContext(options);

        context.Database.EnsureDeleted();
        context.Database.Migrate();

        return context;
    }
}
