namespace Consolidacao.Application.Common.Errors;

public static class ApplicationErrorMessages
{
    public static class Saldo
    {
        public const string TipoInvalido = "O tipo do lançamento deve ser 'Credito' ou 'Debito'.";
        public const string ValorInvalido = "O valor do lançamento deve ser maior que zero.";
        public const string LancamentoIdObrigatorio = "O identificador do lançamento é obrigatório.";
    }
}
