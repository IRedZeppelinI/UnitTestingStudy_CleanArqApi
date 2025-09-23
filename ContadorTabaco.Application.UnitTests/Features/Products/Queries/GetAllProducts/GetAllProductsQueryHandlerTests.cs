using AutoMapper;
using Castle.Core.Logging;
using ContadorTabaco.Application.Common.Mappings;
using ContadorTabaco.Application.Features.Orders.Queries.GetAllOrders;
using ContadorTabaco.Application.Features.Products.Dtos;
using ContadorTabaco.Application.Features.Products.Queries.GetAllProducts;
using ContadorTabaco.Application.Interfaces;
using ContadorTabaco.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions; // MapperConfiguration tem como 2º arg o logger, daí este import.
using Moq;

namespace ContadorTabaco.Application.UnitTests.Features.Products.Queries.GetAllProducts;

public class GetAllProductsQueryHandlerTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IProductRepository> _mockProductRepository;

    public GetAllProductsQueryHandlerTests()
    {

        // ARRANGE (Setup comum no construtor)
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            //cfg.AddProfile<MappingProfile>();
            cfg.AddProfile(new MappingProfile());
        },
        NullLoggerFactory.Instance);

        _mapper = mapperConfig.CreateMapper();
        _mockProductRepository = new Mock<IProductRepository>();
    }

    [Fact]
    public async Task Handle_WhenProductsExist_MustReturnListOfProductsDTO()
    {
        // --- ARRANGE (Preparação específica do teste) ---

        // 1. Criamos os dados falsos que o repositório deveria devolver.
        var productsFromDb = new List<Product>
        {
            new() { Id = 1, Name = "Produto A", Price = 10.0m },
            new() { Id = 2, Name = "Produto B", Price = 20.0m }
        };

        // 2. Configuramos o nosso mock para devolver esses dados.
        _mockProductRepository.Setup(repo => repo
            .GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(productsFromDb);

        // 3. Criamos a instância da classe que vamos testar (System Under Test - SUT).
        var handler = new GetAllProductsQueryHandler(_mockProductRepository.Object, _mapper);


        // --- ACT (Ação) ---
        // 4. Executamos o método que queremos testar.
        var result = await handler.Handle(new GetAllProductsQuery(), new CancellationToken());


        // --- ASSERT (Verificação) ---
        // 5. Verificamos se o resultado é o esperado.
        // Usando xUnit Assert (sem FluentAssertions)
        Assert.NotNull(result);
        Assert.IsType<List<ProductDto>>(result);
        Assert.Equal(2, result.Count);

        var firstProductDto = result.First();
        Assert.Equal(productsFromDb.First().Name, firstProductDto.Name);
        Assert.Equal(productsFromDb.First().Price, firstProductDto.Price);

        /*
        // Como seria com FluentAssertions (para referência futura)
        
        using FluentAssertions;
        result.Should().NotBeNull();
        result.Should().BeOfType<List<ProductDto>>();
        result.Should().HaveCount(2);
        result.First().Name.Should().Be("Produto A");
        */

    }
}
