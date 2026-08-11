namespace Lancamentos.Domain.Lancamentos.Errors;

public static class LancamentoErrors
{
    public const string DescricaoObrigatoria = "A descrição do lançamento é obrigatória.";
    public const string DescricaoTamanhoMinimo = "A descrição deve ter no mínimo 3 caracteres.";
    public const string ValorInvalido = "O valor do lançamento deve ser maior que zero.";
    public const string DataFutura = "A data do lançamento não pode ser no futuro.";
}
