using Consolidacao.Domain.Common;
using Consolidacao.Domain.Saldos;
using Consolidacao.Domain.Saldos.Errors;
using FluentAssertions;

namespace Consolidacao.Domain.Tests;

public class SaldoDiarioTests
{
    [Fact]
    public void Criar_DeveIniciarComTotaisZerados()
    {
        // Arrange
        var data = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var saldo = SaldoDiario.Criar(data);

        // Assert
        saldo.Data.Should().Be(data);
        saldo.TotalCreditos.Should().Be(0m);
        saldo.TotalDebitos.Should().Be(0m);
        saldo.Saldo.Should().Be(0m);
    }

    [Fact]
    public void Aplicar_DeveSomarCredito_ERecalcularSaldo()
    {
        // Arrange
        var saldo = SaldoDiario.Criar(DateOnly.FromDateTime(DateTime.UtcNow));

        // Act
        saldo.Aplicar(TipoLancamento.Credito, 100m);
        saldo.Aplicar(TipoLancamento.Credito, 50m);

        // Assert
        saldo.TotalCreditos.Should().Be(150m);
        saldo.TotalDebitos.Should().Be(0m);
        saldo.Saldo.Should().Be(150m);
    }

    [Fact]
    public void Aplicar_DeveSomarDebito_ERecalcularSaldo()
    {
        // Arrange
        var saldo = SaldoDiario.Criar(DateOnly.FromDateTime(DateTime.UtcNow));

        // Act
        saldo.Aplicar(TipoLancamento.Debito, 40m);

        // Assert
        saldo.TotalDebitos.Should().Be(40m);
        saldo.Saldo.Should().Be(-40m);
    }

    [Fact]
    public void Aplicar_DeveCalcularSaldoMisto_CreditosEDebitos()
    {
        // Arrange
        var saldo = SaldoDiario.Criar(DateOnly.FromDateTime(DateTime.UtcNow));

        // Act
        saldo.Aplicar(TipoLancamento.Credito, 300m);
        saldo.Aplicar(TipoLancamento.Debito, 120m);

        // Assert
        saldo.TotalCreditos.Should().Be(300m);
        saldo.TotalDebitos.Should().Be(120m);
        saldo.Saldo.Should().Be(180m);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Aplicar_DeveLancar_QuandoValorInvalido(decimal valor)
    {
        // Arrange
        var saldo = SaldoDiario.Criar(DateOnly.FromDateTime(DateTime.UtcNow));

        // Act
        Action act = () => saldo.Aplicar(TipoLancamento.Credito, valor);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage(SaldoDiarioErrors.ValorInvalido);
    }
}
