using Consolidacao.Application.Common.Results;
using Consolidacao.Application.Features.Saldos;
using Consolidacao.Application.Features.Saldos.Queries.GetSaldoDiario;
using Consolidacao.Application.Features.Saldos.Queries.ListSaldosDiarios;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Consolidacao.Api.Controllers;

/// <summary>
/// Consulta do saldo diário consolidado (créditos, débitos e saldo por dia).
/// </summary>
[ApiController]
[Route("api/saldos-diarios")]
[Produces("application/json")]
public class SaldosDiariosController : ControllerBase
{
    private readonly ISender _sender;

    public SaldosDiariosController(ISender sender) => _sender = sender;

    /// <summary>Lista os saldos diários dentro de um período opcional.</summary>
    /// <response code="200">Lista retornada com sucesso.</response>
    /// <response code="400">Período informado é inválido.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<SaldoDiarioDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> List(
        [FromQuery] DateOnly? dataInicial,
        [FromQuery] DateOnly? dataFinal,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ListSaldosDiariosRequest(dataInicial, dataFinal), cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : MapFailure(result.Error);
    }

    /// <summary>Consulta o saldo consolidado de um dia específico.</summary>
    /// <remarks>Dias sem nenhum lançamento retornam saldo zerado, não um erro 404.</remarks>
    /// <response code="200">Saldo do dia retornado com sucesso.</response>
    [HttpGet("{data}")]
    [ProducesResponseType(typeof(SaldoDiarioDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByData(DateOnly data, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetSaldoDiarioRequest(data), cancellationToken);

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
