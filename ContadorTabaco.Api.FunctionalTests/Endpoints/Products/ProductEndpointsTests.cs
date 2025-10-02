
using ContadorTabaco.Application.Features.Products.Commands.CreateProduct;
using ContadorTabaco.Application.Features.Products.Dtos;
using ContadorTabaco.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace ContadorTabaco.Api.FunctionalTests.Endpoints.Products;

// Atributo do xUnit que agrupa esta classe de testes na coleção "ApiFunctionalTests".
// Todos os testes de classes diferentes que pertençam à mesma coleção são executados de
// forma SEQUENCIAL, e não em paralelo. Isto é essencial para testes funcionais que
// partilham um recurso externo (como a mesma base de dados de teste), evitando assim
// condições de corrida (race conditions) e deadlocks.
[Collection("ApiFunctionalTests")]
public class ProductEndpointsTests : IClassFixture<CustomWebApplicationFactory>, IDisposable // 1. Implementamos a interface IClassFixture, passando a nossa fábrica personalizada.
{
    // 2. O HttpClient será usado para fazer os pedidos HTTP à nossa API em memória.
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    // 3. O xUnit vai criar UMA instância da nossa fábrica e injetá-la aqui.
    public ProductEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    /*
     *  Explicação:

        IClassFixture<CustomWebApplicationFactory>: Isto diz ao xUnit: "Antes de executares qualquer teste nesta classe,
            por favor, cria uma única instância da CustomWebApplicationFactory. 
            Mantém-na viva enquanto os testes desta classe correm, e no fim, destrói-a."

        HttpClient: O factory.CreateClient() dá-nos um cliente HTTP já configurado para enviar pedidos para o nosso servidor de teste
            em memória.

        Construtor: O xUnit injeta a instância partilhada da fábrica no construtor, que a usamos para inicializar o nosso _client.
     *
     */


    private async Task ResetDatabaseAsync()
    {
        // 1. Criamos um "scope" de serviços para obter um DbContext
        //    Isto garante que estamos a usar a mesma configuração de DI que a nossa app usa
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<Infrastructure.Persistence.AppDbContext>();
        // 2. Limpamos e migramos a base de dados para garantir um estado limpo
        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();
    }

    [Fact]
    [Trait("Category", "Functional")]

    public async Task GetById_WhenProductExists_MustReturn200OkAndCorrectProduct()
    {
        // --- ARRANGE ---        

        // 1. Garantimos um estado limpo chamando o nosso  método para tal.
        await ResetDatabaseAsync();

        // 1. Criamos um "scope" de serviços para obter um DbContext
        //    Isto garante que estamos a usar a mesma configuração de DI que a nossa app usa
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<Infrastructure.Persistence.AppDbContext>();

        

        // 3. Populamos a base de dados com os dados exatos para este teste
        var produtoParaAdicionar = new Product { Name = "Produto de Teste", Price = 9.99m };
        await context.Products.AddAsync(produtoParaAdicionar);
        await context.SaveChangesAsync();

        // --- ACT ---

        // 4. Fazemos um pedido HTTP GET real ao nosso endpoint
        var response = await _client.GetAsync($"/api/Products/{produtoParaAdicionar.Id}");


        // --- ASSERT ---

        // 5. Verificamos a resposta
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // 6. Deserializamos o corpo da resposta e verificamos o seu conteúdo
        var productDto = await response.Content.ReadFromJsonAsync<ProductDto>();
        productDto.Should().NotBeNull();
        productDto.Id.Should().Be(produtoParaAdicionar.Id);
        productDto.Name.Should().Be("Produto de Teste");

        //Com Assert em vez de FluentValidation:
        //Assert.NotNull(productDto);
        //Assert.Equal(productDto.Id, produtoParaAdicionar.Id);
        //Assert.Equal("Produto de Teste", productDto.Name);
            
    }

    [Fact]
    [Trait("Category", "Functional")]

    public async Task GetById_WhenProduct_NOT_Exists_MustReturn404NotFound()
    {
        // --- ARRANGE ---        

        // 1. Garantimos um estado limpo chamando o nosso  método para tal.
        await ResetDatabaseAsync();

                      

        // --- ACT ---

        // 4. Fazemos um pedido HTTP GET real ao nosso endpoint
        var response = await _client.GetAsync($"/api/Products/{999}"); //talvez instalar mock e usar It.Any int?


        // --- ASSERT ---

        // 5. Verificamos a resposta
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);        

    }


    [Fact]
    [Trait("Category", "Functional")]
    public async Task WhenProductIsCreated_MustReturn201OkAndCreatedAtRoute()
    {
        await ResetDatabaseAsync();

        //using var scope = _factory.Services.CreateScope();
        //var context = scope.ServiceProvider.GetRequiredService<Infrastructure.Persistence.AppDbContext>();

        var newProductCommand = new CreateProductCommand { Name = "Novo Produto Teste", Price = 15.50m };

        var response = await _client.PostAsJsonAsync("/api/Products", newProductCommand);

        // --- ASSERT ---

        // 1. Verificamos o código de estado.
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        // 2. Verificamos o cabeçalho "Location".
        Assert.NotNull(response.Headers.Location);
        Assert.Contains("/api/Products/", response.Headers.Location.OriginalString);

        // 3. Verificamos o corpo da resposta.
        var createdProductDto = await response.Content.ReadFromJsonAsync<ProductDto>();
        Assert.NotNull(createdProductDto);
        Assert.Equal(newProductCommand.Name, createdProductDto.Name);
        Assert.Equal(newProductCommand.Price, createdProductDto.Price);
        Assert.True(createdProductDto.Id > 0, "O ID gerado pela base de dados deve ser maior que 0.");

        // 4. A PROVA FINAL (Verificação direta na base de dados).
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<Infrastructure.Persistence.AppDbContext>();
        var productInDb = await context.Products.FindAsync(createdProductDto.Id);

        Assert.NotNull(productInDb);
        Assert.Equal(newProductCommand.Name, productInDb.Name);
    }

    public void Dispose()
    {
        // Embora a fábrica principal seja gerida pelo xUnit, podemos
        // fazer o dispose do cliente específico que criámos.
        _client.Dispose();
    }
}
