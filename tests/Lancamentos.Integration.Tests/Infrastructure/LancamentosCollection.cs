namespace Lancamentos.Integration.Tests.Infrastructure;

[CollectionDefinition(LancamentosCollection.Name)]
public class LancamentosCollection : ICollectionFixture<IntegrationWebApplicationFactory>
{
    public const string Name = "Lancamentos Integration Tests";
}
