using Consolidacao.Application.Common.Errors;
using Consolidacao.Application.Features.Saldos.Commands.AplicarLancamento;
using FluentAssertions;

namespace Consolidacao.Application.Tests.Features.Commands;

public class AplicarLancamentoValidatorTests
{
    private readonly AplicarLancamentoValidator _validator = new();

    [Fact]
    public async Task Validar_DevePassar_QuandoDadosValidos()
    {
        var request = new AplicarLancamentoRequest(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow), "Credito", 100m);
        var result = await _validator.ValidateAsync(request);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validar_DeveFalhar_QuandoLancamentoIdVazio()
    {
        var request = new AplicarLancamentoRequest(Guid.Empty, DateOnly.FromDateTime(DateTime.UtcNow), "Credito", 100m);
        var result = await _validator.ValidateAsync(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == ApplicationErrorMessages.Saldo.LancamentoIdObrigatorio);
    }

    [Theory]
    [InlineData("Invalido")]
    [InlineData("")]
    public async Task Validar_DeveFalhar_QuandoTipoInvalido(string tipo)
    {
        var request = new AplicarLancamentoRequest(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow), tipo, 100m);
        var result = await _validator.ValidateAsync(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == ApplicationErrorMessages.Saldo.TipoInvalido);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public async Task Validar_DeveFalhar_QuandoValorInvalido(decimal valor)
    {
        var request = new AplicarLancamentoRequest(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow), "Debito", valor);
        var result = await _validator.ValidateAsync(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == ApplicationErrorMessages.Saldo.ValorInvalido);
    }
}
