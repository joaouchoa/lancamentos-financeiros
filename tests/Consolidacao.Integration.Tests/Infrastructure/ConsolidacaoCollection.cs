namespace Consolidacao.Integration.Tests.Infrastructure;

[CollectionDefinition(ConsolidacaoCollection.Name)]
public class ConsolidacaoCollection : ICollectionFixture<IntegrationWebApplicationFactory>
{
    public const string Name = "Consolidacao Integration Tests";
}
