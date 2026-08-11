using FluentAssertions;
using Lancamentos.Application.Features.Lancamentos.Queries.ListLancamentos;

namespace Lancamentos.Application.Tests.Features.Queries;

public class ListLancamentosValidatorTests
{
    private readonly ListLancamentosValidator _validator = new();

    [Fact]
    public async Task Validar_DevePassar_QuandoDadosPadrao()
    {
        var result = await _validator.ValidateAsync(new ListLancamentosRequest());
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validar_DeveFalhar_QuandoPaginaInvalida(int pagina)
    {
        var result = await _validator.ValidateAsync(new ListLancamentosRequest(Pagina: pagina));
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public async Task Validar_DeveFalhar_QuandoTamanhoPaginaForaDoIntervalo(int tamanhoPagina)
    {
        var result = await _validator.ValidateAsync(new ListLancamentosRequest(TamanhoPagina: tamanhoPagina));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validar_DeveFalhar_QuandoTipoInvalido()
    {
        var result = await _validator.ValidateAsync(new ListLancamentosRequest(Tipo: "xyz"));
        result.IsValid.Should().BeFalse();
    }
}
