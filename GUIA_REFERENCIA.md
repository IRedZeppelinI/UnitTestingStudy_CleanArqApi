# GUIA_DE_REFERENCIA.md: Testes Automatizados em .NET com Clean Architecture

Este guia serve como uma referência consolidada das técnicas e padrões de testes automatizados aplicados a uma solução .NET com Clean Architecture, usando o projeto `ContadorTabaco` como exemplo prático.

## Índice

1.  [Filosofia de Testes e a Pirâmide](#1-filosofia-de-testes-e-a-pirâmide)
2.  [Ferramentas Essenciais (O Ecossistema de Testes)](#2-ferramentas-essenciais-o-ecossistema-de-testes)
3.  [Testes Unitários (A Base)](#3-testes-unitários-a-base)
4.  [Testes de Integração (A Camada de Persistência)](#4-testes-de-integração-a-camada-de-persistência)
5.  [Testes Funcionais / API (De Ponta a Ponta)](#5-testes-funcionais--api-de-ponta-a-ponta)
6.  [Executar Testes via Linha de Comandos](#6-executar-testes-via-linha-de-comandos)
7.  [Conclusão e Boas Práticas](#7-conclusão-e-boas-práticas)

---

### 1. Filosofia de Testes e a Pirâmide

O objetivo dos testes automatizados não é apenas encontrar bugs, mas sim **aumentar a confiança** no nosso software. Um bom conjunto de testes funciona como uma **rede de segurança** que nos permite refatorizar, adicionar funcionalidades e fazer deploy com a certeza de que não quebrámos o comportamento existente (prevenção de **regressão**). Testes bem escritos servem também como uma **documentação viva** do comportamento esperado do sistema e incentivam um **melhor design de código** (código testável é, por norma, código bem estruturado).

A **Pirâmide de Testes** é o nosso guia estratégico:

* **Testes Unitários (Base Larga)**: Testam a menor unidade de código (uma classe ou método) em total isolamento. São rápidos, baratos e devem ser numerosos.
* **Testes de Integração (Meio)**: Testam a colaboração entre o nosso código e sistemas externos (ex: base de dados). São mais lentos e devem ser menos numerosos que os unitários.
* **Testes Funcionais/E2E (Topo Estreito)**: Testam um fluxo completo da aplicação da perspetiva do utilizador/cliente. São os mais lentos, mais caros e devem ser os menos numerosos, focando-se nos fluxos críticos.

![Pirâmide de Testes](https://martinfowler.com/bliki/images/testPyramid/test-pyramid.png)

---

### 2. Ferramentas Essenciais (O Ecossistema de Testes)

Para construir a nossa suite de testes, utilizámos um conjunto de ferramentas padrão da indústria:

* **xUnit (`xunit`)**: A framework de execução de testes (*test runner*). É responsável por encontrar e executar os métodos marcados com `[Fact]` ou `[Theory]`.
* **Moq (`Moq`)**: A biblioteca de *mocking*. Essencial para os testes unitários, permite-nos criar "duplos" falsos das nossas dependências (ex: `IProductRepository`) para garantir o isolamento.
* **Assert (do xUnit)**: A ferramenta base para fazer verificações (`Assert.Equal`, `Assert.NotNull`, etc.). Embora existam alternativas mais expressivas (como `Shouldly` ou `AwesomeAssertions`), dominar o `Assert` é fundamental.
* **Entity Framework Core (`Microsoft.EntityFrameworkCore.SqlServer`, `...Tools`)**: Necessário nos testes de integração para interagir com a base de dados de teste.
* **ASP.NET Core Test Host (`Microsoft.AspNetCore.Mvc.Testing`)**: A ferramenta principal para testes funcionais. Contém a `WebApplicationFactory`, que arranca a nossa API em memória.

---

### 3. Testes Unitários (A Base)

* **Objetivo**: Testar a lógica de uma classe em **isolamento**.
* **Projeto Exemplo**: `ContadorTabaco.Application.UnitTests`
* **Princípios**: Padrão **AAA (Arrange, Act, Assert)** e uso intensivo de **Mocks**.

#### Dominar o Moq: `Setup`, `Returns`, `Callback` e `Verify`

O Moq é a nossa ferramenta para criar "duplos" de dependências. Os seus métodos principais são:

* **`new Mock<IDependencia>()`**: Cria o objeto mock.
* **`.Object`**: Obtém a instância falsa da dependência para ser injetada na classe que estamos a testar (o nosso SUT - *System Under Test*).
* **`.Setup(expression)`**: Configura o comportamento de um método ou propriedade do mock. Usado principalmente para "queries" ou métodos que devolvem um valor.
    * **`It.IsAny<T>()`**: Um "argument matcher" que diz ao `Setup` para intercetar a chamada independentemente do valor do argumento, desde que o tipo seja `T`.
* **`.ReturnsAsync(valor)`**: Define o valor que um método `async` deve devolver. O valor é automaticamente embrulhado numa `Task`.
* **`.Callback(action)`**: Permite executar uma ação quando o método do mock é chamado. É perfeito para simular efeitos secundários, como a atribuição de um ID pela base de dados.
* **`.Verify(expression, times)`**: Usado na fase `Assert` para verificar se um método no mock foi chamado. É a ferramenta principal para testar *commands* e garantir que os efeitos secundários ocorreram. `Times.Once()` garante que foi chamado exatamente uma vez.

#### Exemplo 1: Testar uma Query (`GetProductByIdQueryHandler`)
```csharp
// Em ...Application.UnitTests/.../GetProductByIdQueryHandlerTests.cs
[Fact]
public async Task Handle_WhenProductExist_MustReturnProductsDTO()
{
    // --- ARRANGE ---
    var productToReturn = new Product { Id = 1, Name = "Produto A", Price = 10.0m };

    _mockProductRepository
        .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
        .ReturnsAsync(productToReturn);

    var handler = new GetProductByIdQueryHandler(_mockProductRepository.Object, _mapper);

    // --- ACT ---
    var result = await handler.Handle(new GetProductByIdQuery(1), CancellationToken.None);

    // --- ASSERT ---
    Assert.NotNull(result);
    Assert.IsType<ProductDto>(result);
    Assert.Equal(productToReturn.Name, result.Name);
}
````

#### Exemplo 2: Testar um Command (`CreateProductCommandHandler`)

```csharp
// Em ...Application.UnitTests/.../CreateProductCommandHandlerTests.cs
[Fact]
public async Task Handle_QuandoComandoEValido_DeveAdicionarProdutoERetornarId()
{
    // --- ARRANGE ---
    var command = new CreateProductCommand { Name = "Produto A", Price = 10.0m };
    var expectedId = 123;

    _productRepository
        .Setup(repo => repo.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
        .Callback<Product, CancellationToken>((product, ct) => product.Id = expectedId); // Simula a geração de ID

    var handler = new CreateProductCommandHandler(_productRepository.Object, _unitOfWork.Object, _mapper);

    // --- ACT ---
    var result = await handler.Handle(command, CancellationToken.None);

    // --- ASSERT ---
    Assert.Equal(expectedId, result);

    _unitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
}
```

-----

### 4\. Testes de Integração (A Camada de Persistência)

  * **Objetivo**: Testar a colaboração entre o nosso código (`Repository`, `DbContext`, Configurações de Entidade) e a base de dados.
  * **Projeto Exemplo**: `ContadorTabaco.Infrastructure.IntegrationTests`

#### O Setup: `DbContextFactory`

A `DbContextFactory` é uma classe auxiliar estática com uma única responsabilidade: fornecer uma instância do `AppDbContext` ligada a uma base de dados de teste **limpa e com o schema atualizado**.

  * **Mecanismo**:
    1.  Lê uma `connection string` de um `appsettings.json` dedicado aos testes.
    2.  Configura o `DbContext` para usar o `SQL Server`.
    3.  Chama `context.Database.EnsureDeleted()` para apagar qualquer base de dados de uma execução anterior, garantindo **isolamento**.
    4.  Chama `context.Database.Migrate()` para aplicar todas as migrações existentes, garantindo que o **schema está correto**.

#### O Setup: Gestão de Paralelismo com `[Collection]`

Como cada classe de teste de integração chama a `DbContextFactory` (que faz operações destrutivas na BD), não podemos permitir que elas corram em paralelo.

  * **`DatabaseTestCollection.cs`**: Uma classe vazia que serve apenas para definir o nome de uma coleção.
    ```csharp
    [CollectionDefinition("DatabaseTests")]
    public class DatabaseTestCollection { }
    ```
  * **`[Collection("DatabaseTests")]`**: Atributo colocado no topo de cada classe de teste de integração para as agrupar. O xUnit garante que todas as classes na mesma coleção são executadas de forma **sequencial**.

#### Exemplo: Testar o Repositório (`OrderRepositoryTests`)

Este teste valida que o método `GetByIdAsync` do `OrderRepository` funciona corretamente e que a configuração da entidade `Order` está a carregar os dados relacionados (`Product`) através do `.Include()`.

```csharp
[Collection("DatabaseTests")]
public class OrderRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    public OrderRepositoryTests() { _context = DbContextFactory.Create(); }

    [Fact]
    public async Task GetByIdAsync_QuandoOrderExiste_DeveRetornarOrderComProduct()
    {
        // --- ARRANGE ---
        var productToAdd = new Product { Name = "Marlboro Red", Price = 5.20m };
        await _context.Products.AddAsync(productToAdd);
        await _context.SaveChangesAsync(); 

        var orderToAdd = Order.Create(productToAdd, 2);
        await _context.Orders.AddAsync(orderToAdd);
        await _context.SaveChangesAsync();

        var repository = new OrderRepository(_context);

        // --- ACT ---
        var result = await repository.GetByIdAsync(orderToAdd.Id, CancellationToken.None);

        // --- ASSERT ---
        Assert.NotNull(result);
        Assert.Equal(orderToAdd.Id, result.Id);
        Assert.NotNull(result.Product); // Verifica se o Include funcionou
        Assert.Equal(productToAdd.Name, result.Product.Name);
    }
    
    public void Dispose() => _context.Dispose();
}
```

-----

### 5\. Testes Funcionais / API (De Ponta a Ponta)

  * **Objetivo**: Testar um fluxo completo da aplicação, desde um pedido HTTP até à resposta, passando por todas as camadas.
  * **Projeto Exemplo**: `ContadorTabaco.Api.FunctionalTests`

#### Pré-requisito: Expor o Ponto de Entrada da API

Para que a `WebApplicationFactory` saiba qual aplicação arrancar, a classe `Program` do projeto `Api` precisa de ser visível. Adicionamos uma classe parcial vazia no fim do `Program.cs`.

```csharp
// No fim de Api/Program.cs
public partial class Program { }
```

#### O Setup: `CustomWebApplicationFactory`

Esta é a peça central. É uma classe que herda de `WebApplicationFactory<Program>` e tem a responsabilidade de arrancar a nossa API em memória, mas com serviços **substituídos** para o ambiente de teste.

  * **Mecanismo**: Sobrescreve `ConfigureWebHost` para aceder ao `IServiceCollection` antes de a aplicação arrancar. Aí, remove o registo original do `DbContext` e adiciona um novo, configurado para usar a base de dados de teste.

#### O Setup: A Classe de Teste (`IClassFixture` e Isolamento)

  * **`IClassFixture<CustomWebApplicationFactory>`**: Interface do xUnit que diz ao *runner* para criar **uma única instância** da `CustomWebApplicationFactory` (um recurso caro) e partilhá-la por todos os testes na mesma classe.
  * **Isolamento de Dados**: Como a base de dados é partilhada entre os testes da mesma classe, cada teste individual é responsável por garantir um estado limpo, chamando um método auxiliar no início da sua fase `Arrange` para apagar e migrar a base de dados.

#### Exemplo: Testar um `POST` Endpoint (`ProductEndpointsTests`)

```csharp
[Collection("ApiFunctionalTests")]
public class ProductEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public ProductEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
    
    private async Task ResetDatabaseAsync() 
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<Infrastructure.Persistence.AppDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();
    }

    [Fact]
    public async Task Create_QuandoComandoEValido_DeveRetornar201CreatedComProduto()
    {
        // --- ARRANGE ---
        await ResetDatabaseAsync();
        var command = new CreateProductCommand { Name = "Novo Produto", Price = 15.50m };

        // --- ACT ---
        var response = await _client.PostAsJsonAsync("/api/Products", command);

        // --- ASSERT ---
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var dto = await response.Content.ReadFromJsonAsync<ProductDto>();
        Assert.NotNull(dto);
        Assert.Equal(command.Name, dto.Name);
        
        // Verificação final na base de dados para garantir a persistência
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<Infrastructure.Persistence.AppDbContext>();
        var productInDb = await context.Products.FindAsync(dto.Id);
        Assert.NotNull(productInDb);
    }
}
```

-----

### 6\. Executar Testes via Linha de Comandos

O comando `dotnet test` é a ferramenta principal. A opção `--filter` (ou `-f`) permite isolar testes.

| Objetivo | Comando `dotnet test` |
| :--- | :--- |
| Correr todos os testes | `dotnet test` |
| Correr testes num projeto | `dotnet test <caminho_para.csproj>` |
| Correr testes cujo nome contém... | `--filter "DisplayName~<ParteDoNome>"` |
| Correr por categoria (melhor prática) | `--filter "Category=<NomeDaCategoria>"` |

Para usar o filtro por categoria, decora os teus testes com o atributo `[Trait("Category", "Unit")]` ou `[Trait("Category", "Integration")]`, etc.

-----

### 7\. Conclusão e Boas Práticas

  * **Testar no Sítio Certo**: Testa a lógica de negócio em testes unitários, a persistência em testes de integração e os fluxos completos em testes funcionais.
  * **Isolamento é Rei**: Garante que os teus testes não dependem uns dos outros. Limpa o estado (base de dados, ficheiros, etc.) antes de cada teste ou conjunto de testes.
  * **Testes como Documentação**: Nomes de teste descritivos e asserções claras fazem com que os teus testes documentem o comportamento esperado do sistema.
  * **Confia na Pirâmide**: Escreve muitos testes unitários rápidos, alguns testes de integração mais lentos e poucos testes funcionais/E2E para os fluxos mais críticos.
  * **DRY vs. Isolamento**: No código de teste, a clareza e o isolamento são muitas vezes mais importantes do que evitar a todo o custo a repetição de código (DRY).

