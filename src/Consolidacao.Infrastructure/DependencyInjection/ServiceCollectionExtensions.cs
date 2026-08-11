using Consolidacao.Domain.Saldos.Repositories;
using Consolidacao.Infrastructure.Messaging.Consumers;
using Consolidacao.Infrastructure.Persistence;
using Consolidacao.Infrastructure.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Consolidacao.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ConsolidacaoDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Postgres")));

        services.AddScoped<ISaldoDiarioRepository, SaldoDiarioRepository>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<LancamentoRegistradoConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var host = configuration["RabbitMq:Host"] ?? "localhost";
                var port = int.TryParse(configuration["RabbitMq:Port"], out var parsedPort) ? parsedPort : 5672;

                cfg.Host(new Uri($"rabbitmq://{host}:{port}/"), h =>
                {
                    h.Username(configuration["RabbitMq:Username"] ?? "guest");
                    h.Password(configuration["RabbitMq:Password"] ?? "guest");
                });

                cfg.ReceiveEndpoint("consolidacao-lancamento-registrado", e =>
                {
                    // Poucas tentativas com backoff — mensagens que continuam falhando
                    // vão para a fila de erro em vez de retry indefinido, priorizando
                    // a saúde do consumidor sob carga sobre reprocessar tudo a qualquer custo.
                    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                    e.ConfigureConsumer<LancamentoRegistradoConsumer>(context);
                });
            });
        });

        return services;
    }
}
