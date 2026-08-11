namespace Consolidacao.Domain.Saldos;

/// <summary>
/// Cópia local do tipo de lançamento, na linguagem ubíqua do contexto de
/// Consolidação. Não referencia o enum do serviço de Lançamentos — cada
/// serviço tem seu próprio modelo, o acoplamento entre eles é só o contrato
/// de mensagem em Shared.Contracts.
/// </summary>
public enum TipoLancamento
{
    Credito,
    Debito
}
