using FluentAssertions;
using Lancamentos.Application.Common.Errors;
using Lancamentos.Application.Features.Lancamentos.Commands.RegistrarLancamento;

namespace Lancamentos.Application.Tests.Features.Commands;

public class RegistrarLancamentoValidatorTests
{
    private readonly RegistrarLancamentoValidator _validator = new();

    [Fact]
    public async Task Validar_DevePassar_QuandoDadosValidos()
    {
        var request = new RegistrarLancamentoRequest(DateOnly.FromDateTime(DateTime.UtcNow), "Credito", 100m, "Venda");
        var result = await _validator.ValidateAsync(request);
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validar_DeveFalhar_QuandoDescricaoVazia(string descricao)
    {
        var request = new RegistrarLancamentoRequest(DateOnly.FromDateTime(DateTime.UtcNow), "Credito", 100m, descricao);
        var result = await _validator.ValidateAsync(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == ApplicationErrorMessages.Lancamento.DescricaoObrigatoria);
    }

    [Fact]
    public async Task Validar_DeveFalhar_QuandoDescricaoMenorQueMinimo()
    {
        var request = new RegistrarLancamentoRequest(DateOnly.FromDateTime(DateTime.UtcNow), "Credito", 100m, "ab");
        var result = await _validator.ValidateAsync(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == ApplicationErrorMessages.Lancamento.DescricaoTamanhoMinimo);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public async Task Validar_DeveFalhar_QuandoValorInvalido(decimal valor)
    {
        var request = new RegistrarLancamentoRequest(DateOnly.FromDateTime(DateTime.UtcNow), "Credito", valor, "Venda");
        var result = await _validator.ValidateAsync(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == ApplicationErrorMessages.Lancamento.ValorInvalido);
    }

    [Theory]
    [InlineData("Invalido")]
    [InlineData("")]
    public async Task Validar_DeveFalhar_QuandoTipoInvalido(string tipo)
    {
        var request = new RegistrarLancamentoRequest(DateOnly.FromDateTime(DateTime.UtcNow), tipo, 100m, "Venda");
        var result = await _validator.ValidateAsync(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == ApplicationErrorMessages.Lancamento.TipoInvalido);
    }

    [Fact]
    public async Task Validar_DeveFalhar_QuandoDataDefault()
    {
        var request = new RegistrarLancamentoRequest(default, "Credito", 100m, "Venda");
        var result = await _validator.ValidateAsync(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == ApplicationErrorMessages.Lancamento.DataObrigatoria);
    }
}
