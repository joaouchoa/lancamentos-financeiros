using System.Reflection;
using Consolidacao.Infrastructure.Persistence;
using DbUp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace Consolidacao.Integration.Tests.Infrastructure;

public class IntegrationWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string RabbitMqUsername = "guest";
    private const string RabbitMqPassword = "guest";

    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .Build();

    private readonly RabbitMqContainer _rabbitMq = new RabbitMqBuilder()
        .WithImage("rabbitmq:3-management-alpine")
        .WithUsername(RabbitMqUsername)
        .WithPassword(RabbitMqPassword)
        .Build();

    public async Task InitializeAsync()
    {
        await Task.WhenAll(_postgres.StartAsync(), _rabbitMq.StartAsync());
        RunMigrations();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["RabbitMq:Host"] = _rabbitMq.Hostname,
                ["RabbitMq:Port"] = _rabbitMq.GetMappedPublicPort(5672).ToString(),
                ["RabbitMq:Username"] = RabbitMqUsername,
                ["RabbitMq:Password"] = RabbitMqPassword
            });
        });

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ConsolidacaoDbContext>));

            if (descriptor is not null)
                services.Remove(descriptor);

            services.AddDbContext<ConsolidacaoDbContext>(options =>
                options.UseNpgsql(_postgres.GetConnectionString()));
        });

        builder.UseEnvironment("Testing");
    }

    private void RunMigrations()
    {
        var upgrader = DeployChanges.To
            .PostgresqlDatabase(_postgres.GetConnectionString())
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .LogToConsole()
            .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
            throw new InvalidOperationException($"Migração falhou: {result.Error}");
    }

    public new async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
        await _rabbitMq.DisposeAsync();
        await base.DisposeAsync();
    }
}
