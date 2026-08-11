using FluentValidation;

namespace Consolidacao.Application.Features.Saldos.Queries.ListSaldosDiarios;

public sealed class ListSaldosDiariosValidator : AbstractValidator<ListSaldosDiariosRequest>
{
    public ListSaldosDiariosValidator()
    {
        When(x => x.DataInicial.HasValue && x.DataFinal.HasValue, () =>
        {
            RuleFor(x => x.DataFinal)
                .GreaterThanOrEqualTo(x => x.DataInicial)
                .WithMessage("A data final deve ser maior ou igual à data inicial.");
        });
    }
}
