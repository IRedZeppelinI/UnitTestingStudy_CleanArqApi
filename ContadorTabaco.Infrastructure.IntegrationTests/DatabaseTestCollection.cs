using Xunit;

namespace ContadorTabaco.Infrastructure.IntegrationTests;

/// <summary>
/// Esta classe serve como uma "definição" para uma coleção de testes do xUnit
/// com o nome "DatabaseTests".
///
/// A classe em si não precisa de conter nenhum código. O seu único propósito
/// é hospedar o atributo [CollectionDefinition].
///
/// Todas as classes de teste que forem decoradas com o atributo [Collection("DatabaseTests")]
/// serão agrupadas. O xUnit garante que todos os testes de uma mesma coleção
/// são executados de forma sequencial, prevenindo a execução em paralelo.
///
/// Isto é crucial para os nossos testes de integração de infraestrutura, pois todos
/// eles manipulam a mesma base de dados e não podem correr em simultâneo
/// para evitar deadlocks durante a criação/eliminação da base de dados.
/// </summary>
[CollectionDefinition("DatabaseTests")]
public class DatabaseTestCollection
{
}