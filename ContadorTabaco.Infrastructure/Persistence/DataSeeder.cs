using ContadorTabaco.Domain.Entities;

namespace ContadorTabaco.Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Só executa o seed se não houver produtos na base de dados.
        if (context.Products.Any())
        {
            return; // A base de dados já foi populada
        }

        // 1. Criar os produtos sem definir os IDs.
        var productMarlboroRed = new Product { Name = "Marlboro Red", Price = 5.20m };
        var productCamelBlue = new Product { Name = "Camel Blue", Price = 5.00m };
        var productGoldenVirginia = new Product { Name = "Golden Virginia 30g", Price = 6.10m };
        var productLmBlue = new Product { Name = "L&M Blue", Price = 4.80m };
        var productIqosHeets = new Product { Name = "IQOS Heets Amber", Price = 4.50m };

        // 2. Adicionar os produtos ao contexto.
        await context.Products.AddRangeAsync(
            productMarlboroRed,
            productCamelBlue,
            productGoldenVirginia,
            productLmBlue,
            productIqosHeets
        );

        // 3. Criar as encomendas, usando as propriedades de navegação.
        var orders = new List<Order>
        {
            new() { Quantity = 2, Product = productMarlboroRed, TotalCost = 2 * 5.20m, OrderDate = new DateTime(2025, 9, 15) },
            new() { Quantity = 1, Product = productGoldenVirginia, TotalCost = 1 * 6.10m, OrderDate = new DateTime(2025, 9, 16) },
            new() { Quantity = 5, Product = productIqosHeets, TotalCost = 5 * 4.50m, OrderDate = new DateTime(2025, 9, 17) },
            new() { Quantity = 1, Product = productMarlboroRed, TotalCost = 1 * 5.20m, OrderDate = new DateTime(2025, 9, 18) }
        };

        // 4. Adicionar as encomendas ao contexto.
        await context.Orders.AddRangeAsync(orders);

        // 5. Gravar tudo numa única transação. O EF Core gere todas as chaves.
        await context.SaveChangesAsync();
    }
}