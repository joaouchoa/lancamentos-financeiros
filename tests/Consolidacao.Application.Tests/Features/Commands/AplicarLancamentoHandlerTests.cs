using Consolidacao.Application.Features.Saldos.Commands.AplicarLancamento;
using Consolidacao.Domain.Saldos;
using Consolidacao.Domain.Saldos.Repositories;
using FluentAssertions;
using NSubstitute;

namespace Consolidacao.Application.Tests.Features.Commands;

public class AplicarLancamentoHandlerTests
{
    private readonly ISaldoDiarioRepository _repository;
    private readonly AplicarLancamentoHandler _handler;

    public AplicarLancamentoHandlerTests()
    {
        _repository = Substitute.For<ISaldoDiarioRepository>();
        _handler = new AplicarLancamentoHandler(_repository);
    }

    [Fact]
    public async Task Handle_DeveCriarNovoSaldoDiario_QuandoNaoExisteParaAData()
    {
        // Arrange
        var data = DateOnly.FromDateTime(DateTime.UtcNow);
        _repository.LancamentoJaProcessadoAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);
        _repository.GetByDataAsync(data, Arg.Any<CancellationToken>()).Returns((SaldoDiario?)null);

        var request = new AplicarLancamentoRequest(Guid.NewGuid(), data, "Credito", 100m);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Aplicado.Should().BeTrue();
        await _repository.Received(1).AddAsync(Arg.Is<SaldoDiario>(s => s.Saldo == 100m), Arg.Any<CancellationToken>());
        await _repository.Received(1).MarcarLancamentoProcessadoAsync(request.LancamentoId, Arg.Any<CancellationToken>());
        await _repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_DeveAtualizarSaldoExistente_QuandoJaExisteParaAData()
    {
        // Arrange
        var data = DateOnly.FromDateTime(DateTime.UtcNow);
        var saldoExistente = SaldoDiario.Criar(data);
        saldoExistente.Aplicar(TipoLancamento.Credito, 200m);

        _repository.LancamentoJaProcessadoAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);
        _repository.GetByDataAsync(data, Arg.Any<CancellationToken>()).Returns(saldoExistente);

        var request = new AplicarLancamentoRequest(Guid.NewGuid(), data, "Debito", 50m);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        saldoExistente.TotalCreditos.Should().Be(200m);
        saldoExistente.TotalDebitos.Should().Be(50m);
        saldoExistente.Saldo.Should().Be(150m);
        await _repository.DidNotReceive().AddAsync(Arg.Any<SaldoDiario>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_DeveSerIdempotente_QuandoLancamentoJaFoiProcessado()
    {
        // Arrange
        var lancamentoId = Guid.NewGuid();
        _repository.LancamentoJaProcessadoAsync(lancamentoId, Arg.Any<CancellationToken>()).Returns(true);

        var request = new AplicarLancamentoRequest(lancamentoId, DateOnly.FromDateTime(DateTime.UtcNow), "Credito", 100m);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Aplicado.Should().BeFalse();
        await _repository.DidNotReceive().GetByDataAsync(Arg.Any<DateOnly>(), Arg.Any<CancellationToken>());
        await _repository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
