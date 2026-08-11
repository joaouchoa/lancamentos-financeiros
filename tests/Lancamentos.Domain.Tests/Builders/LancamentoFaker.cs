using Bogus;
using Lancamentos.Domain.Lancamentos;

namespace Lancamentos.Domain.Tests.Builders;

public class LancamentoFaker : Faker<Lancamento>
{
    public LancamentoFaker()
    {
        CustomInstantiator(f =>
        {
            var data = DateOnly.FromDateTime(f.Date.Recent(10));
            var tipo = f.PickRandom<TipoLancamento>();
            var valor = f.Random.Decimal(1m, 1000m);
            var descricao = f.Commerce.ProductName();
            return Lancamento.Criar(data, tipo, valor, descricao);
        });
    }
}
