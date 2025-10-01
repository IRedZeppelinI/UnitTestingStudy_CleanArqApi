using ContadorTabaco.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ContadorTabaco.Api.FunctionalTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Este método permite-nos personalizar a configuração do servidor de teste.
        builder.ConfigureServices(services =>
        {
            // 1. Encontrar e remover a configuração original do AppDbContext
            //    que está registada no Program.cs da API.
            services.RemoveAll<DbContextOptions<AppDbContext>>();

            // 2. Adicionar uma nova configuração do AppDbContext, mas desta vez
            //    a apontar para a nossa base de dados de teste isolada.
            services.AddDbContext<AppDbContext>(options =>
            {
                // 1. Construir um provedor de configuração para ler o nosso appsettings.json de teste
                var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();
                // 2. Obter a connection string do ficheiro
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(connectionString);
            });
        });
    }

}
