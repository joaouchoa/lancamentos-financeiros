using Lancamentos.Domain.Common;
using Lancamentos.Domain.Lancamentos.Errors;

namespace Lancamentos.Domain.Lancamentos;

/// <summary>
/// Representa um lançamento de crédito ou débito no fluxo de caixa.
/// É imutável após criado — correções de negócio são feitas com um novo
/// lançamento de estorno, nunca editando um lançamento existente.
/// </summary>
public class Lancamento : Entity
{
    public DateOnly Data { get; private set; }
    public TipoLancamento Tipo { get; private set; }
    public decimal Valor { get; private set; }
    public string Descricao { get; private set; } = null!;

    private Lancamento() { }

    private Lancamento(DateOnly data, TipoLancamento tipo, decimal valor, string descricao)
    {
        Data = data;
        Tipo = tipo;
        Valor = valor;
        Descricao = descricao;
    }

    public static Lancamento Criar(DateOnly data, TipoLancamento tipo, decimal valor, string descricao)
    {
        Validar(data, valor, descricao);
        return new Lancamento(data, tipo, valor, descricao.Trim());
    }

    private static void Validar(DateOnly data, decimal valor, string descricao)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            throw new DomainException(LancamentoErrors.DescricaoObrigatoria);

        if (descricao.Trim().Length < 3)
            throw new DomainException(LancamentoErrors.DescricaoTamanhoMinimo);

        if (valor <= 0)
            throw new DomainException(LancamentoErrors.ValorInvalido);

        if (data > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new DomainException(LancamentoErrors.DataFutura);
    }
}
