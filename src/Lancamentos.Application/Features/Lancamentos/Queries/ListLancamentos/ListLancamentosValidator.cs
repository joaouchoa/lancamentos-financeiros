using FluentValidation;
using Lancamentos.Domain.Lancamentos;

namespace Lancamentos.Application.Features.Lancamentos.Queries.ListLancamentos;

public sealed class ListLancamentosValidator : AbstractValidator<ListLancamentosRequest>
{
    public ListLancamentosValidator()
    {
        RuleFor(x => x.Pagina)
            .GreaterThan(0).WithMessage("A página deve ser maior que zero.");

        RuleFor(x => x.TamanhoPagina)
            .InclusiveBetween(1, 100).WithMessage("O tamanho da página deve estar entre 1 e 100.");

        RuleFor(x => x.Tipo)
            .Must(tipo => string.IsNullOrWhiteSpace(tipo) || Enum.TryParse<TipoLancamento>(tipo, ignoreCase: true, out _))
            .WithMessage("O tipo informado no filtro deve ser 'Credito' ou 'Debito'.");
    }
}
