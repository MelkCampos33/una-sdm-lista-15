using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GreenDriveApi20260101.Data;

namespace GreenDriveApi20260101.Controllers;

[ApiController]
[Route("api/intelligence")]
public class GridIntelligenceController : ControllerBase
{
    private readonly AppDbContext _context;

    public GridIntelligenceController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/intelligence/carbon-footprint
    /// <summary>
    /// Agregação distribuída: Retorna o custo total de reciclagem agrupado por cidade.
    /// Simula latência de rede IoT via MQTT/HTTP lento (3 segundos de delay).
    /// </summary>
    [HttpGet("carbon-footprint")]
    public async Task<IActionResult> GetCarbonFootprint()
    {
        // Simulação de latência de rede IoT (MQTT/HTTP lento)
        Console.WriteLine("[GridIntelligence] Consultando sensores IoT em campo... (simulando latência MQTT)");
        await Task.Delay(3000);

        // Agregação distribuída: agrupar CustoProcessamento por Cidade da Estação
        var resultado = await _context.OrdensReciclagem
            .Include(o => o.Estacao)
            .GroupBy(o => o.Estacao!.Localizacao)
            .Select(g => new
            {
                Cidade = g.Key,
                TotalOrdensReciclagem = g.Count(),
                CustoTotalProcessamento = g.Sum(o => o.CustoProcessamento),
                CustoMedioProcessamento = g.Average(o => (double)o.CustoProcessamento),
                BateriasRecicladas = g.Select(o => o.BateriaId).Distinct().Count()
            })
            .OrderByDescending(r => r.CustoTotalProcessamento)
            .ToListAsync();

        Console.WriteLine($"[GridIntelligence] Dados agregados de {resultado.Count} cidade(s).");

        return Ok(new
        {
            mensagem = "Pegada de carbono por cidade (dados agregados via IoT)",
            totalCidades = resultado.Count,
            latenciaSimuladaMs = 3000,
            protocoloSimulado = "MQTT/HTTP",
            dados = resultado
        });
    }

    // GET: api/intelligence/dashboard
    /// <summary>
    /// Painel de resumo geral do ecossistema GreenDrive.
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var totalBaterias = await _context.Baterias.CountAsync();
        var bateriasInativas = await _context.Baterias.CountAsync(b => b.SaudeBateria <= 10);
        var bateriasSecondLife = await _context.Baterias.CountAsync(b => b.SaudeBateria > 60);
        var totalEstacoes = await _context.EstacoesCarga.CountAsync();
        var totalTelemetrias = await _context.RegistrosTelemetria.CountAsync();
        var totalOrdens = await _context.OrdensReciclagem.CountAsync();
        var custoTotalReciclagem = await _context.OrdensReciclagem.SumAsync(o => o.CustoProcessamento);

        return Ok(new
        {
            resumoGeral = new
            {
                totalBaterias,
                bateriasInativas,
                bateriasAptas_SecondLife = bateriasSecondLife,
                totalEstacoesCarga = totalEstacoes,
                totalRegistrosTelemetria = totalTelemetrias,
                totalOrdensReciclagem = totalOrdens,
                custoTotalProcessamentoR = custoTotalReciclagem
            }
        });
    }
}
