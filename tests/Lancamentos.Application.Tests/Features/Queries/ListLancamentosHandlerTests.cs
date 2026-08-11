using FluentAssertions;
using Lancamentos.Application.Features.Lancamentos.Queries.ListLancamentos;
using Lancamentos.Application.Tests.Fakers;
using Lancamentos.Domain.Lancamentos;
using Lancamentos.Domain.Lancamentos.Repositories;
using NSubstitute;

namespace Lancamentos.Application.Tests.Features.Queries;

public class ListLancamentosHandlerTests
{
    private readonly ILancamentoRepository _repository;
    private readonly ListLancamentosHandler _handler;

    public ListLancamentosHandlerTests()
    {
        _repository = Substitute.For<ILancamentoRepository>();
        _handler = new ListLancamentosHandler(_repository);
    }

    [Fact]
    public async Task Handle_DeveRetornarPaginaComTotalCalculado()
    {
        // Arrange
        var lancamentos = new LancamentoFaker().Generate(3);
        _repository.ListAsync(Arg.Any<LancamentoFilter>(), Arg.Any<CancellationToken>()).Returns(lancamentos);
        _repository.CountAsync(Arg.Any<LancamentoFilter>(), Arg.Any<CancellationToken>()).Returns(23);

        var request = new ListLancamentosRequest(Pagina: 1, TamanhoPagina: 10);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Itens.Should().HaveCount(3);
        result.Value.TotalRegistros.Should().Be(23);
        result.Value.TotalPaginas.Should().Be(3);
    }

    [Fact]
    public async Task Handle_DeveIgnorarFiltroDeTipo_QuandoInvalido()
    {
        // Arrange
        _repository.ListAsync(Arg.Any<LancamentoFilter>(), Arg.Any<CancellationToken>())
            .Returns([]);
        _repository.CountAsync(Arg.Any<LancamentoFilter>(), Arg.Any<CancellationToken>())
            .Returns(0);

        var request = new ListLancamentosRequest(Tipo: "invalido");

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        await _repository.Received(1).ListAsync(
            Arg.Is<LancamentoFilter>(f => f.Tipo == null), Arg.Any<CancellationToken>());
    }
}
