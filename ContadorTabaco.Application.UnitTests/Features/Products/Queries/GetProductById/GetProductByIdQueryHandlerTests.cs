using AutoMapper;
using ContadorTabaco.Application.Common.Mappings;
using ContadorTabaco.Application.Features.Products.Dtos;
using ContadorTabaco.Application.Features.Products.Queries.GetProductById;
using ContadorTabaco.Application.Interfaces;
using ContadorTabaco.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace ContadorTabaco.Application.UnitTests.Features.Products.Queries.GetProductById;

public class GetProductByIdQueryHandlerTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IProductRepository> _mockProductRepository;

    public GetProductByIdQueryHandlerTests()
    {
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        },NullLoggerFactory.Instance);

        _mapper = mapperConfig.CreateMapper();
        _mockProductRepository = new Mock<IProductRepository>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Handle_WhenProductExist_MustReturnProductsDTO()
    {      

        var productsFromDb1 = new Product { Id = 1, Name = "Produto A", Price = 10.0m };
        var productsFromDb2 = new Product { Id = 2, Name = "Produto B", Price = 20.0m };


        _mockProductRepository.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(productsFromDb1);
        _mockProductRepository.Setup(repo => repo.GetByIdAsync(2, It.IsAny<CancellationToken>())).ReturnsAsync(productsFromDb2);


        var handler = new GetProductByIdQueryHandler(_mockProductRepository.Object, _mapper);

        var result = await handler.Handle(new GetProductByIdQuery(2), new CancellationToken());

        Assert.NotNull(result);
        Assert.IsType<ProductDto>(result);
        Assert.Equal(productsFromDb2.Name, result.Name);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Handle_WhenProduct_NOT_Exist_MustReturnNull()
    {


        _mockProductRepository.Setup(repo => repo
            .GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<Product?>(null)); //ou ReturnsAsync((Product?)null); 


        var handler = new GetProductByIdQueryHandler(_mockProductRepository.Object, _mapper);

        var result = await handler.Handle(new GetProductByIdQuery(999), new CancellationToken());

        Assert.Null(result);
    }
}
