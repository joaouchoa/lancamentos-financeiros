using Lancamentos.Domain.Common;
using Lancamentos.Domain.Lancamentos;
using Lancamentos.Domain.Lancamentos.Errors;
using Lancamentos.Domain.Tests.Builders;
using FluentAssertions;

namespace Lancamentos.Domain.Tests;

public class LancamentoTests
{
    [Fact]
    public void Criar_DeveCriarLancamento_QuandoDadosValidos()
    {
        // Arrange
        var data = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var lancamento = Lancamento.Criar(data, TipoLancamento.Credito, 150.75m, "Venda de mercadoria");

        // Assert
        lancamento.Data.Should().Be(data);
        lancamento.Tipo.Should().Be(TipoLancamento.Credito);
        lancamento.Valor.Should().Be(150.75m);
        lancamento.Descricao.Should().Be("Venda de mercadoria");
    }

    [Fact]
    public void Criar_DeveApararEspacos_NaDescricao()
    {
        // Act
        var lancamento = new LancamentoFaker().Generate();
        var comEspacos = Lancamento.Criar(lancamento.Data, lancamento.Tipo, lancamento.Valor, "  Pagamento de fornecedor  ");

        // Assert
        comEspacos.Descricao.Should().Be("Pagamento de fornecedor");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_DeveLancar_QuandoDescricaoVazia(string descricao)
    {
        // Act
        Action act = () => Lancamento.Criar(DateOnly.FromDateTime(DateTime.UtcNow), TipoLancamento.Credito, 100m, descricao);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage(LancamentoErrors.DescricaoObrigatoria);
    }

    [Fact]
    public void Criar_DeveLancar_QuandoDescricaoMenorQueMinimo()
    {
        // Act
        Action act = () => Lancamento.Criar(DateOnly.FromDateTime(DateTime.UtcNow), TipoLancamento.Debito, 100m, "ab");

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage(LancamentoErrors.DescricaoTamanhoMinimo);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Criar_DeveLancar_QuandoValorInvalido(decimal valor)
    {
        // Act
        Action act = () => Lancamento.Criar(DateOnly.FromDateTime(DateTime.UtcNow), TipoLancamento.Debito, valor, "Compra de insumos");

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage(LancamentoErrors.ValorInvalido);
    }

    [Fact]
    public void Criar_DeveLancar_QuandoDataNoFuturo()
    {
        // Arrange
        var dataFutura = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1);

        // Act
        Action act = () => Lancamento.Criar(dataFutura, TipoLancamento.Credito, 100m, "Recebimento futuro");

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage(LancamentoErrors.DataFutura);
    }

    [Fact]
    public void Criar_DeveAceitar_QuandoDataEHoje()
    {
        // Arrange
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var lancamento = Lancamento.Criar(hoje, TipoLancamento.Debito, 50m, "Pagamento à vista");

        // Assert
        lancamento.Data.Should().Be(hoje);
    }
}
