using Lancamentos.Application.Common.Results;
using Lancamentos.Application.Features.Lancamentos.Commands.RegistrarLancamento;
using Lancamentos.Application.Features.Lancamentos.Queries.GetLancamentoById;
using Lancamentos.Application.Features.Lancamentos.Queries.ListLancamentos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Lancamentos.Api.Controllers;

/// <summary>
/// Gestão de lançamentos financeiros (créditos e débitos) do fluxo de caixa.
/// </summary>
[ApiController]
[Route("api/lancamentos")]
[Produces("application/json")]
public class LancamentosController : ControllerBase
{
    private readonly ISender _sender;

    public LancamentosController(ISender sender) => _sender = sender;

    /// <summary>Registra um novo lançamento (crédito ou débito).</summary>
    /// <response code="201">Lançamento registrado com sucesso.</response>
    /// <response code="400">Dados de entrada inválidos.</response>
    [HttpPost]
    [ProducesResponseType(typeof(RegistrarLancamentoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Registrar(
        [FromBody] RegistrarLancamentoRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(request, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value)
            : MapFailure(result.Error);
    }

    /// <summary>Lista lançamentos com filtros opcionais e paginação.</summary>
    /// <response code="200">Lista retornada com sucesso.</response>
    /// <response code="400">Parâmetros de filtro inválidos.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ListLancamentosPagedResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> List(
        [FromQuery] DateOnly? data,
        [FromQuery] string? tipo,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10,
        CancellationToken cancellationToken = default)
    {
        var request = new ListLancamentosRequest(data, tipo, pagina, tamanhoPagina);
        var result = await _sender.Send(request, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : MapFailure(result.Error);
    }

    /// <summary>Busca um lançamento pelo Id.</summary>
    /// <response code="200">Lançamento encontrado.</response>
    /// <response code="404">Lançamento não encontrado.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Application.Features.Lancamentos.LancamentoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetLancamentoByIdRequest(id), cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : MapFailure(result.Error);
    }

    private IActionResult MapFailure(Error error) => error.Code switch
    {
        "NotFound" => NotFound(new { error.Message }),
        "Conflict" => Conflict(new { error.Message }),
        "Validation" => UnprocessableEntity(new { error.Message }),
        _ => BadRequest(new { error.Message })
    };
}
