using Consolidacao.Application.Common.Errors;
using Consolidacao.Domain.Saldos;
using FluentValidation;

namespace Consolidacao.Application.Features.Saldos.Commands.AplicarLancamento;

public sealed class AplicarLancamentoValidator : AbstractValidator<AplicarLancamentoRequest>
{
    public AplicarLancamentoValidator()
    {
        RuleFor(x => x.LancamentoId)
            .NotEmpty().WithMessage(ApplicationErrorMessages.Saldo.LancamentoIdObrigatorio);

        RuleFor(x => x.Tipo)
            .Must(tipo => Enum.TryParse<TipoLancamento>(tipo, ignoreCase: true, out _))
            .WithMessage(ApplicationErrorMessages.Saldo.TipoInvalido);

        RuleFor(x => x.Valor)
            .GreaterThan(0).WithMessage(ApplicationErrorMessages.Saldo.ValorInvalido);
    }
}
