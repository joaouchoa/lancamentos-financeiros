using Consolidacao.Application.Features.Saldos.Queries.ListSaldosDiarios;
using FluentAssertions;

namespace Consolidacao.Application.Tests.Features.Queries;

public class ListSaldosDiariosValidatorTests
{
    private readonly ListSaldosDiariosValidator _validator = new();

    [Fact]
    public async Task Validar_DevePassar_QuandoSemFiltros()
    {
        var result = await _validator.ValidateAsync(new ListSaldosDiariosRequest());
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validar_DeveFalhar_QuandoDataFinalMenorQueDataInicial()
    {
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var request = new ListSaldosDiariosRequest(hoje, hoje.AddDays(-1));

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validar_DevePassar_QuandoDataFinalIgualDataInicial()
    {
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var request = new ListSaldosDiariosRequest(hoje, hoje);

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
    }
}
