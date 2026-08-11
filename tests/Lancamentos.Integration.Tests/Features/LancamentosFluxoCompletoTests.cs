using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Lancamentos.Integration.Tests.Common.Dtos;
using Lancamentos.Integration.Tests.Infrastructure;
using FluentAssertions;

namespace Lancamentos.Integration.Tests.Features;

[Collection(LancamentosCollection.Name)]
public class LancamentosFluxoCompletoTests
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public LancamentosFluxoCompletoTests(IntegrationWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task FluxoCompleto_RegistrarConsultarELisrar_DeveExecutarComSucesso()
    {
        // ── 1. Registrar lançamento de crédito ──────────────────────────────
        var registrarBody = new
        {
            Data = DateOnly.FromDateTime(DateTime.UtcNow),
            Tipo = "Credito",
            Valor = 500.00m,
            Descricao = "Venda de mercadoria — teste de integração"
        };

        var registrarResponse = await _client.PostAsJsonAsync("/api/lancamentos", registrarBody);

        registrarResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        registrarResponse.Headers.Location.Should().NotBeNull();

        var lancamento = await registrarResponse.Content.ReadFromJsonAsync<LancamentoDto>(JsonOptions);
        lancamento!.Tipo.Should().Be("Credito");
        lancamento.Valor.Should().Be(500.00m);

        // ── 2. Buscar por Id ─────────────────────────────────────────────────
        var getResponse = await _client.GetAsync($"/api/lancamentos/{lancamento.Id}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var lancamentoDetalhe = await getResponse.Content.ReadFromJsonAsync<LancamentoDto>(JsonOptions);
        lancamentoDetalhe!.Id.Should().Be(lancamento.Id);
        lancamentoDetalhe.Descricao.Should().Be("Venda de mercadoria — teste de integração");

        // ── 3. Listar com filtro por tipo ────────────────────────────────────
        var listResponse = await _client.GetAsync("/api/lancamentos?tipo=Credito");

        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var pagina = await listResponse.Content.ReadFromJsonAsync<PagedLancamentosDto>(JsonOptions);
        pagina!.Itens.Should().Contain(l => l.Id == lancamento.Id);
    }

    [Fact]
    public async Task Registrar_DeveRetornar400_QuandoDescricaoVazia()
    {
        var body = new { Data = DateOnly.FromDateTime(DateTime.UtcNow), Tipo = "Debito", Valor = 50m, Descricao = "" };

        var response = await _client.PostAsJsonAsync("/api/lancamentos", body);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Registrar_DeveRetornar422_QuandoDataNoFuturo()
    {
        var body = new
        {
            Data = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(5),
            Tipo = "Credito",
            Valor = 100m,
            Descricao = "Recebimento futuro"
        };

        var response = await _client.PostAsJsonAsync("/api/lancamentos", body);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task GetById_DeveRetornar404_QuandoLancamentoNaoExiste()
    {
        var response = await _client.GetAsync($"/api/lancamentos/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
