using Consolidacao.Application.Features.Saldos.Queries.GetSaldoDiario;
using Consolidacao.Domain.Saldos;
using Consolidacao.Domain.Saldos.Repositories;
using FluentAssertions;
using NSubstitute;

namespace Consolidacao.Application.Tests.Features.Queries;

public class GetSaldoDiarioHandlerTests
{
    private readonly ISaldoDiarioRepository _repository;
    private readonly GetSaldoDiarioHandler _handler;

    public GetSaldoDiarioHandlerTests()
    {
        _repository = Substitute.For<ISaldoDiarioRepository>();
        _handler = new GetSaldoDiarioHandler(_repository);
    }

    [Fact]
    public async Task Handle_DeveRetornarSaldo_QuandoExisteParaAData()
    {
        // Arrange
        var data = DateOnly.FromDateTime(DateTime.UtcNow);
        var saldo = SaldoDiario.Criar(data);
        saldo.Aplicar(TipoLancamento.Credito, 300m);
        saldo.Aplicar(TipoLancamento.Debito, 100m);

        _repository.GetByDataAsync(data, Arg.Any<CancellationToken>()).Returns(saldo);

        // Act
        var result = await _handler.Handle(new GetSaldoDiarioRequest(data), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalCreditos.Should().Be(300m);
        result.Value.TotalDebitos.Should().Be(100m);
        result.Value.Saldo.Should().Be(200m);
    }

    [Fact]
    public async Task Handle_DeveRetornarZerado_QuandoNaoExisteSaldoParaAData()
    {
        // Arrange
        var data = DateOnly.FromDateTime(DateTime.UtcNow);
        _repository.GetByDataAsync(data, Arg.Any<CancellationToken>()).Returns((SaldoDiario?)null);

        // Act
        var result = await _handler.Handle(new GetSaldoDiarioRequest(data), CancellationToken.None);

        // Assert — dia sem lançamentos é um estado válido, não um erro
        result.IsSuccess.Should().BeTrue();
        result.Value!.Data.Should().Be(data);
        result.Value.Saldo.Should().Be(0m);
    }
}
