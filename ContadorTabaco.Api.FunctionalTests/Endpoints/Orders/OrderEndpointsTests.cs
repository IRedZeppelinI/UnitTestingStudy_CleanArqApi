
using ContadorTabaco.Application.Features.Orders.Dtos;
using ContadorTabaco.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace ContadorTabaco.Api.FunctionalTests.Endpoints.Orders;

[Collection("ApiFunctionalTests")]
public class OrderEndpointsTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public OrderEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

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
    public async Task GetById_WhenOrderExists_MustReturn200OkAndCorrectOrder()
    {
        await ResetDatabaseAsync();

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<Infrastructure.Persistence.AppDbContext>();

        var productToAdd = new Product { Name = "Marlboro Red", Price = 5.20m };
        await context.AddAsync(productToAdd);

        var orderToAdd = Order.Create(productToAdd, 2, new DateTime(2025, 9, 15));
        await context.AddAsync(orderToAdd);
        await context.SaveChangesAsync();

        var response = await _client.GetAsync($"/api/Orders/{orderToAdd.Id}");


        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var orderDto = await response.Content.ReadFromJsonAsync<OrderDto>();
        orderDto.Should().NotBeNull();
        orderDto.Id.Should().Be(orderToAdd.Id);
        orderDto.ProductId.Should().Be(productToAdd.Id);
    }
      
    


    public void Dispose()
    {
        _client.Dispose();
    }
}
