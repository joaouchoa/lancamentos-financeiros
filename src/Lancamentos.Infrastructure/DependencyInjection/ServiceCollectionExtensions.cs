using Lancamentos.Application.Common.Outbox;
using Lancamentos.Domain.Lancamentos.Repositories;
using Lancamentos.Infrastructure.Outbox;
using Lancamentos.Infrastructure.Persistence;
using Lancamentos.Infrastructure.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lancamentos.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<LancamentosDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Postgres")));

        services.AddScoped<ILancamentoRepository, LancamentoRepository>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();

        services.AddHostedService<OutboxPublisher>();

        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                var host = configuration["RabbitMq:Host"] ?? "localhost";
                var port = int.TryParse(configuration["RabbitMq:Port"], out var parsedPort) ? parsedPort : 5672;

                cfg.Host(new Uri($"rabbitmq://{host}:{port}/"), h =>
                {
                    h.Username(configuration["RabbitMq:Username"] ?? "guest");
                    h.Password(configuration["RabbitMq:Password"] ?? "guest");
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
