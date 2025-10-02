using ContadorTabaco.Domain.Entities;
using ContadorTabaco.Infrastructure.Persistence;
using ContadorTabaco.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;
using System.Net;

namespace ContadorTabaco.Infrastructure.IntegrationTests.Repositories;

[Collection("DatabaseTests")]
public class ProductRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;

    public ProductRepositoryTests()
    {
        _context = DbContextFactory.Create();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetByIdAsync_WhenProductExists_MustReturnCorrectProduct()
    {
        // --- ARRANGE (Preparar) ---

        // 1. Criamos a entidade que queremos guardar na nossa BD de teste.
        var produtoParaAdicionar = new Product { Name = "Produto de Teste", Price = 9.99m };
        await _context.Products.AddAsync(produtoParaAdicionar);
        await _context.SaveChangesAsync(); // Guardamos na BD SQL Server de teste.

        // 2. Criamos a instância do repositório que vamos testar, passando o contexto já com dados.
        var repository = new ProductRepository(_context);

        // --- ACT (Ação) ---

        // 3. Executamos o método que queremos testar, usando o ID que o EF Core gerou.
        var result = await repository.GetByIdAsync(produtoParaAdicionar.Id, CancellationToken.None);

        // --- ASSERT (Verificação) ---

        // 4. Verificamos se o resultado é o que esperávamos.
        Assert.NotNull(result);
        Assert.Equal(produtoParaAdicionar.Id, result.Id);
        Assert.Equal("Produto de Teste", result.Name);

        /*
        // Como seria com FluentAssertions:
        using FluentAssertions;
        result.Should().NotBeNull();
        result.Id.Should().Be(produtoParaAdicionar.Id);
        result.Name.Should().Be("Produto de Teste");
        */
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllAsync_WhenProductExists_MustReturnProductList()
    {
        // --- ARRANGE (Preparar) ---

        var productsToAdd = new List<Product>
        {
            new() { Name = "Produto A", Price = 10.0m },
            new() { Name = "Produto B", Price = 20.0m }
        };
        await _context.Products.AddRangeAsync(productsToAdd);
        await _context.SaveChangesAsync(); 

        
        var repository = new ProductRepository(_context);

        // --- ACT (Ação) ---
        
        var result = await repository.GetAllAsync(CancellationToken.None);

        // --- ASSERT (Verificação) ---
        
        Assert.NotNull(result);
        Assert.IsType<List<Product>>(result);
        Assert.Equal("Produto A", result.First().Name);

        /*
        // Como seria com FluentAssertions:
        using FluentAssertions;
        result.Should().NotBeNull();
        result.Should().BeOfType<List<Product>>();
        result.First().Name.Should().Be("Produto A");
        */
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllAsync_WhenProduct_NOT_Exists_MustReturnEmptyProductList()
    {
        // --- ARRANGE (Preparar) ---
                

        
        var repository = new ProductRepository(_context);

        // --- ACT (Ação) ---
        
        var result = await repository.GetAllAsync(CancellationToken.None);

        // --- ASSERT (Verificação) ---
        
        Assert.NotNull(result);
        Assert.IsType<List<Product>>(result);
        Assert.Empty(result);

        /*
        // Como seria com FluentAssertions:
        using FluentAssertions;
        result.Should().NotBeNull();
        result.Should().BeOfType<List<Product>>();
        
        */
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task AddedProductIsOnDatabase()
    {
        var repository = new ProductRepository(_context);
        var unitOfWork = new UnitOfWork(_context);

        var productsToAdd = new Product { Name = "Produto A", Price = 10.0m };
        
        await repository.AddAsync(productsToAdd, CancellationToken.None);
        await unitOfWork.SaveChangesAsync(CancellationToken.None);

        var result = await _context.Products.FirstAsync();

        Assert.NotNull(result);
        Assert.IsType<Product>(result);
        Assert.Equal(productsToAdd.Name, result.Name);
    }


    public void Dispose()
    {
        // Este método é chamado automaticamente no fim da execução dos testes
        // e garante que a nossa conexão à BD é fechada e os recursos libertados.
        _context.Dispose();
    }
}
