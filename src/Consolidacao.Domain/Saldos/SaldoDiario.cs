using Consolidacao.Domain.Common;
using Consolidacao.Domain.Saldos.Errors;

namespace Consolidacao.Domain.Saldos;

/// <summary>
/// Consolidado diário de créditos e débitos. Representa o saldo referente a
/// um único dia — não é um saldo acumulado ao longo do tempo.
/// </summary>
public class SaldoDiario : Entity
{
    public DateOnly Data { get; private set; }
    public decimal TotalCreditos { get; private set; }
    public decimal TotalDebitos { get; private set; }
    public decimal Saldo { get; private set; }

    private SaldoDiario() { }

    private SaldoDiario(DateOnly data)
    {
        Data = data;
        TotalCreditos = 0m;
        TotalDebitos = 0m;
        Saldo = 0m;
    }

    public static SaldoDiario Criar(DateOnly data) => new(data);

    public void Aplicar(TipoLancamento tipo, decimal valor)
    {
        if (valor <= 0)
            throw new DomainException(SaldoDiarioErrors.ValorInvalido);

        if (tipo == TipoLancamento.Credito)
            TotalCreditos += valor;
        else
            TotalDebitos += valor;

        RecalcularSaldo();
    }

    private void RecalcularSaldo() => Saldo = TotalCreditos - TotalDebitos;
}
