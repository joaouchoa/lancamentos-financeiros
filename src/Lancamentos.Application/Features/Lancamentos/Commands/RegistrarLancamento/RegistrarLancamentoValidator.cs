using FluentValidation;
using Lancamentos.Application.Common.Errors;
using Lancamentos.Domain.Lancamentos;

namespace Lancamentos.Application.Features.Lancamentos.Commands.RegistrarLancamento;

public sealed class RegistrarLancamentoValidator : AbstractValidator<RegistrarLancamentoRequest>
{
    public RegistrarLancamentoValidator()
    {
        RuleFor(x => x.Data)
            .NotEqual(default(DateOnly))
            .WithMessage(ApplicationErrorMessages.Lancamento.DataObrigatoria);

        RuleFor(x => x.Tipo)
            .Must(tipo => Enum.TryParse<TipoLancamento>(tipo, ignoreCase: true, out _))
            .WithMessage(ApplicationErrorMessages.Lancamento.TipoInvalido);

        RuleFor(x => x.Descricao)
            .NotEmpty().WithMessage(ApplicationErrorMessages.Lancamento.DescricaoObrigatoria)
            .MinimumLength(3).WithMessage(ApplicationErrorMessages.Lancamento.DescricaoTamanhoMinimo);

        RuleFor(x => x.Valor)
            .GreaterThan(0).WithMessage(ApplicationErrorMessages.Lancamento.ValorInvalido);
    }
}
