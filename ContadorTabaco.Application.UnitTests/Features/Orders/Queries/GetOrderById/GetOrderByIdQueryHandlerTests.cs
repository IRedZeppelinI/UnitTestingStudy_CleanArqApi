using AutoMapper;
using ContadorTabaco.Application.Common.Mappings;
using ContadorTabaco.Application.Features.Orders.Dtos;
using ContadorTabaco.Application.Features.Orders.Queries.GetOrderById;
using ContadorTabaco.Application.Interfaces;
using ContadorTabaco.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace ContadorTabaco.Application.UnitTests.Features.Orders.Queries.GetOrderById;

public class GetOrderByIdQueryHandlerTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IOrderRepository> _orderRepository;

    public GetOrderByIdQueryHandlerTests()
    {
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        }, NullLoggerFactory.Instance);
        _mapper = mapperConfig.CreateMapper();
        _orderRepository = new Mock<IOrderRepository>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Handle_WhenOrderExist_MustReturnOrderDTO()
    {
        var productsFromDb = new Product { Id = 1, Name = "Produto A", Price = 10.0m };
        var orderFromDb = Order.Create(productsFromDb, 3, new DateTime(2025, 9, 15));
        orderFromDb.Id = 101;

        _orderRepository.Setup(repo => repo.GetByIdAsync(orderFromDb.Id, It.IsAny<CancellationToken>())).ReturnsAsync(orderFromDb);

        var handler = new GetOrderByIdQueryHandler(_orderRepository.Object, _mapper);

        var result = await handler.Handle(new GetOrderByIdQuery(orderFromDb.Id), CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<OrderDto>(result);
        Assert.Equal(orderFromDb.Id, result.Id);
        Assert.Equal(orderFromDb.Quantity, result.Quantity);
        Assert.Equal(orderFromDb.TotalCost, result.TotalCost);
        Assert.Equal(orderFromDb.Product.Name, result.ProductName);

        /*
        // Com FluentAssertions
        using FluentAssertions;
        result.Should().NotBeNull();
        result.Should().BeOfType<OrderDto>();
        result.Id.Should().Be(orderToReturn.Id);
        result.Quantity.Should().Be(orderToReturn.Quantity);
        result.ProductName.Should().Be(orderToReturn.Product.Name);
        */
    }
}
