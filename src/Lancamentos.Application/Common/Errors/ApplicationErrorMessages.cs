namespace Lancamentos.Application.Common.Errors;

public static class ApplicationErrorMessages
{
    public static class Lancamento
    {
        public const string LancamentoNaoEncontrado = "Lançamento não encontrado.";
        public const string DataObrigatoria = "A data do lançamento é obrigatória.";
        public const string TipoInvalido = "O tipo do lançamento deve ser 'Credito' ou 'Debito'.";
        public const string DescricaoObrigatoria = "A descrição do lançamento é obrigatória.";
        public const string DescricaoTamanhoMinimo = "A descrição deve ter no mínimo 3 caracteres.";
        public const string ValorInvalido = "O valor do lançamento deve ser maior que zero.";
    }
}
