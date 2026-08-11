using FluentAssertions;
using Lancamentos.Application.Common.Outbox;
using Lancamentos.Application.Features.Lancamentos.Commands.RegistrarLancamento;
using Lancamentos.Domain.Lancamentos;
using Lancamentos.Domain.Lancamentos.Repositories;
using NSubstitute;

namespace Lancamentos.Application.Tests.Features.Commands;

public class RegistrarLancamentoHandlerTests
{
    private readonly ILancamentoRepository _repository;
    private readonly IOutboxRepository _outboxRepository;
    private readonly RegistrarLancamentoHandler _handler;

    public RegistrarLancamentoHandlerTests()
    {
        _repository = Substitute.For<ILancamentoRepository>();
        _outboxRepository = Substitute.For<IOutboxRepository>();
        _handler = new RegistrarLancamentoHandler(_repository, _outboxRepository);
    }

    [Fact]
    public async Task Handle_DeveRetornarSucesso_QuandoDadosValidos()
    {
        // Arrange
        var request = new RegistrarLancamentoRequest(
            DateOnly.FromDateTime(DateTime.UtcNow), "Credito", 250m, "Venda de produto");

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Tipo.Should().Be("Credito");
        result.Value.Valor.Should().Be(250m);
        result.Value.Descricao.Should().Be("Venda de produto");
    }

    [Fact]
    public async Task Handle_DevePersistirLancamentoEGravarOutboxNaMesmaOperacao()
    {
        // Arrange
        var request = new RegistrarLancamentoRequest(
            DateOnly.FromDateTime(DateTime.UtcNow), "Debito", 100m, "Pagamento de fornecedor");

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        await _repository.Received(1).AddAsync(Arg.Any<Lancamento>(), Arg.Any<CancellationToken>());
        await _outboxRepository.Received(1).AddAsync(Arg.Any<Lancamento>(), Arg.Any<CancellationToken>());
        await _repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_DeveLancarTipoCorretamente_QuandoTipoDebito()
    {
        // Arrange
        var request = new RegistrarLancamentoRequest(
            DateOnly.FromDateTime(DateTime.UtcNow), "debito", 100m, "Compra de insumos");

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Value!.Tipo.Should().Be("Debito");
    }
}
