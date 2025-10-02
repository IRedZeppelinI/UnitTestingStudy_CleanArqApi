using AutoMapper;
using ContadorTabaco.Application.Common.Mappings;
using ContadorTabaco.Application.Features.Products.Commands.CreateProduct;
using ContadorTabaco.Application.Interfaces;
using ContadorTabaco.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace ContadorTabaco.Application.UnitTests.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandlerTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IProductRepository> _productRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;

    public CreateProductCommandHandlerTests()
    {
        var mappingConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        }, NullLoggerFactory.Instance);

        _mapper = mappingConfig.CreateMapper();
        _productRepository = new Mock<IProductRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
    }

    [Fact]
    public async Task When_HandlerCreatesProduct_MustReturnId()
    {

        var createProductCommand = new CreateProductCommand { Name = "Produto A", Price = 10.0m };

        // Criamos uma entidade Product que esperamos que seja "adicionada"
        var productEntity = new Product { Name = createProductCommand.Name, Price = createProductCommand.Price };
        var expectedProductId = 123;

        _productRepository.Setup(repo => repo
            .AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            // O Callback é executado quando o AddAsync é chamado.
            // Ele pega no produto que o handler lhe passou e atribui-lhe um ID.
            .Callback<Product, CancellationToken>((product, ct) =>
            {
                product.Id = expectedProductId; // Simulamos que a BD atribuiu o ID 123
            });

        //_productRepository.Setup(repo => repo
        //    .AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
        //    .Returns(Task.CompletedTask);

        _unitOfWork.Setup(repo => repo
            .SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler =  new CreateProductCommandHandler(_productRepository.Object, _unitOfWork.Object, _mapper);

        var result = await handler.Handle(createProductCommand, CancellationToken.None);


        Assert.Equal(expectedProductId, result);       

        // Verifica se o método AddAsync foi chamado exatamente uma vez,
        // com qualquer objeto do tipo Product.
        _productRepository.Verify(repo => repo
            .AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);

        // Verifica se o método SaveChangesAsync foi chamado exatamente uma vez.
        _unitOfWork.Verify(uow => uow
            .SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        Assert.IsType<int>(result);
    }
}
