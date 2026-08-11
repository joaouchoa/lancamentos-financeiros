using FluentAssertions;
using Lancamentos.Application.Features.Lancamentos.Queries.GetLancamentoById;
using Lancamentos.Application.Tests.Fakers;
using Lancamentos.Domain.Lancamentos;
using Lancamentos.Domain.Lancamentos.Repositories;
using NSubstitute;

namespace Lancamentos.Application.Tests.Features.Queries;

public class GetLancamentoByIdHandlerTests
{
    private readonly ILancamentoRepository _repository;
    private readonly GetLancamentoByIdHandler _handler;

    public GetLancamentoByIdHandlerTests()
    {
        _repository = Substitute.For<ILancamentoRepository>();
        _handler = new GetLancamentoByIdHandler(_repository);
    }

    [Fact]
    public async Task Handle_DeveRetornarSucesso_QuandoLancamentoExiste()
    {
        // Arrange
        var lancamento = new LancamentoFaker().Generate();
        _repository.GetByIdAsync(lancamento.Id, Arg.Any<CancellationToken>()).Returns(lancamento);

        // Act
        var result = await _handler.Handle(new GetLancamentoByIdRequest(lancamento.Id), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(lancamento.Id);
    }

    [Fact]
    public async Task Handle_DeveRetornarFalha_QuandoLancamentoNaoEncontrado()
    {
        // Arrange
        _repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Lancamento?)null);

        // Act
        var result = await _handler.Handle(new GetLancamentoByIdRequest(Guid.NewGuid()), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("NotFound");
    }
}
