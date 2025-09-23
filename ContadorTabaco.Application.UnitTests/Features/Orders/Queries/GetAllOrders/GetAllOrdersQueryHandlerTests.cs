using AutoMapper;
using ContadorTabaco.Application.Common.Mappings;
using ContadorTabaco.Application.Features.Orders.Dtos;
using ContadorTabaco.Application.Features.Orders.Queries.GetAllOrders;
using ContadorTabaco.Application.Interfaces;
using ContadorTabaco.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace ContadorTabaco.Application.UnitTests.Features.Orders.Queries.GetAllOrders;

public class GetAllOrdersQueryHandlerTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IOrderRepository> _mockOrderRepository;

    public GetAllOrdersQueryHandlerTests()
    {
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());

        }, NullLoggerFactory.Instance);

        _mapper = mapperConfig.CreateMapper();
        _mockOrderRepository = new Mock<IOrderRepository>();
    }

    [Fact]
    public async Task Handle_WhenOrdersExist_MustReturnListOfOrdersDto()
    {
        var produtoA = new Product { Id = 1, Name = "Marlboro Red", Price = 5.20m };
        var produtoB = new Product { Id = 2, Name = "Camel Blue", Price = 5.00m };

        var mockOrders = new List<Order>
        {
            new Order()
            {
                Id = 101,
                Quantity = 2,
                TotalCost = 10.40m,
                OrderDate = DateTime.Now.AddDays(-1),
                ProductId = produtoA.Id, 
                Product = produtoA      
            },
            new Order()
            {
                Id = 102,
                Quantity = 3,
                TotalCost = 15.00m,
                OrderDate = DateTime.Now,
                ProductId = produtoB.Id, 
                Product = produtoB      
            }
        };

        _mockOrderRepository.Setup(repo => repo
            .GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockOrders);

        var handler = new GetAllOrdersQueryHandler(_mockOrderRepository.Object, _mapper);

        var result = await handler.Handle(new GetAllOrdersQuery(), new CancellationToken());

        Assert.NotNull(result);
        Assert.IsType<List<OrderDto>>(result);
        Assert.Equal(2, result.Count);

        var firstOrder = result.First();
        Assert.Equal(mockOrders.First().Id, firstOrder.Id);
        Assert.Equal(mockOrders.First().ProductId, firstOrder.ProductId);
        Assert.Equal(mockOrders.First().Product.Name, firstOrder.ProductName);

        //result.Should().NotBeNull();
        //result.Should().BeOfType<List<OrderDto>>();
        //result.Should().HaveCount(2);
        //result.First().Id.Should().Be(1);
    }

    [Fact]
    public async Task Handle_WhenOrders_NOT_Exist_MustReturnEmptyListOfOrdersDto()
    {
        // --- ARRANGE ---
        // A única coisa que muda é que configuramos o mock para devolver uma lista vazia.
        _mockOrderRepository.Setup(repo => repo
            .GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Order>());

        var handler = new GetAllOrdersQueryHandler(_mockOrderRepository.Object, _mapper);

        // --- ACT ---
        var result = await handler.Handle(new GetAllOrdersQuery(), new CancellationToken());

        // --- ASSERT ---
        Assert.NotNull(result);
        Assert.Empty(result); // Um Assert do xUnit específico para verificar listas vazias.
                              // ou Assert.Equal(0, result.Count);
    }

}
