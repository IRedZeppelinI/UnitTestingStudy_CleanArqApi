using ContadorTabaco.Domain.Entities;
using ContadorTabaco.Infrastructure.Persistence;
using ContadorTabaco.Infrastructure.Repositories;
using FluentAssertions;

namespace ContadorTabaco.Infrastructure.IntegrationTests.Repositories;

[Collection("DatabaseTests")]
public class OrderRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;

    public OrderRepositoryTests()
    {
        _context = DbContextFactory.Create();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetByIdAsync_WhenOrderExists_MustReturnCorrectOrderWithProduct()
    {
        var productToAdd = new Product { Name = "Marlboro Red", Price = 5.20m };

        await _context.AddAsync(productToAdd);

        var orderToAdd = Order.Create(productToAdd, 3, new DateTime(2025, 9, 15));

        await _context.AddAsync(orderToAdd);
        await _context.SaveChangesAsync();

        var orderRepository = new OrderRepository(_context);

        var result = await orderRepository.GetByIdAsync(orderToAdd.Id, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(orderToAdd.Id);
        result.ProductId.Should().Be(productToAdd.Id);

        result.Product.Should().NotBeNull(); // Verificamos que o Include funcionou
        result.Product!.Id.Should().Be(productToAdd.Id);
        result.Product.Name.Should().Be("Marlboro Red");
        result.TotalCost.Should().Be(result.Quantity * result.Product.Price);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllAsync_WhenOrderExists_MustReturnOrderList()
    {
        // --- ARRANGE (Preparar) ---

        var productMarlboroRed = new Product { Name = "Marlboro Red", Price = 5.20m };
        var productGoldenVirginia = new Product { Name = "Golden Virginia 30g", Price = 6.10m };

        await _context.Products.AddRangeAsync(
            productMarlboroRed,
            productGoldenVirginia
        );
        
        var ordersToAdd = new List<Order>
        {
            Order.Create(productMarlboroRed, 2, new DateTime(2025, 9, 15)),
            Order.Create(productGoldenVirginia, 1, new DateTime(2025, 9, 16))
        };

        await _context.Orders.AddRangeAsync(ordersToAdd);
        await _context.SaveChangesAsync();

        var orderRepository = new OrderRepository( _context );


        // --- ACT (Ação) ---

        var result = await orderRepository.GetAllAsync(CancellationToken.None);


        // --- ASSERT (Verificação) ---

        Assert.NotNull(result);
        Assert.IsType<List<Order>>(result);
        Assert.Equal(2, result.First().Quantity);
        Assert.Equal(result.First().Quantity * productMarlboroRed.Price, result.First().TotalCost);


        /*
        // Como seria com FluentAssertions:
        using FluentAssertions;
        result.Should().NotBeNull();
        result.Should().BeOfType<List<Order>>();
        */
    }


    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllAsync_WhenOrder_NOT_Exists_MustReturnEmptyOrderList()
    {
        // --- ARRANGE (Preparar) ---



        var repository = new OrderRepository(_context);

        // --- ACT (Ação) ---

        var result = await repository.GetAllAsync(CancellationToken.None);

        // --- ASSERT (Verificação) ---

        Assert.NotNull(result);
        Assert.IsType<List<Order>>(result);
        Assert.Empty(result);

        /*
        // Como seria com FluentAssertions:
        using FluentAssertions;
        result.Should().NotBeNull();
        result.Should().BeOfType<List<Order>>();
        
        */
    }


    public void Dispose()
    {
        // Este método é chamado automaticamente no fim da execução dos testes
        // e garante que a nossa conexão à BD é fechada e os recursos libertados.
        _context.Dispose();
    }
}
