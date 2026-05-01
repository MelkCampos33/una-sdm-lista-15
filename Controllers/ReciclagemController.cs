using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GreenDriveApi20260101.Data;
using GreenDriveApi20260101.Models;

namespace GreenDriveApi20260101.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReciclagemController : ControllerBase
{
    private readonly AppDbContext _context;
    private const decimal TaxaAmbientalUltraRapida = 250.00m;

    public ReciclagemController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/reciclagem
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrdemReciclagem>>> GetOrdens()
    {
        return await _context.OrdensReciclagem
            .Include(o => o.Bateria)
            .Include(o => o.Estacao)
            .ToListAsync();
    }

    // GET: api/reciclagem/5
    [HttpGet("{id}")]
    public async Task<ActionResult<OrdemReciclagem>> GetOrdem(int id)
    {
        var ordem = await _context.OrdensReciclagem
            .Include(o => o.Bateria)
            .Include(o => o.Estacao)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (ordem == null)
            return NotFound(new { mensagem = $"Ordem de reciclagem com ID {id} não encontrada." });

        return ordem;
    }

    // POST: api/reciclagem
    /// <summary>
    /// Cria uma ordem de reciclagem.
    /// REGRA 1: SoH > 60% — bateria apta para Reuso Doméstico (Second Life), não reciclagem.
    /// REGRA 2: Estação Ultra-Rapida adiciona taxa ambiental de R$ 250,00.
    /// INTEGRIDADE: BateriaId e EstacaoId devem referenciar registros existentes.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<OrdemReciclagem>> PostOrdem(OrdemReciclagem ordem)
    {
        // Integridade: Validar existência da Bateria
        var bateria = await _context.Baterias.FindAsync(ordem.BateriaId);
        if (bateria == null)
            return NotFound(new { mensagem = $"Bateria com ID {ordem.BateriaId} não encontrada. A ordem de reciclagem exige uma bateria válida." });

        // Integridade: Validar existência da Estação
        var estacao = await _context.EstacoesCarga.FindAsync(ordem.EstacaoId);
        if (estacao == null)
            return NotFound(new { mensagem = $"Estação com ID {ordem.EstacaoId} não encontrada. A ordem de reciclagem exige uma estação válida." });

        // Regra de Sustentabilidade: SoH > 60% -> Second Life
        if (bateria.SaudeBateria > 60)
        {
            return BadRequest(new
            {
                mensagem = $"Bateria com saúde superior a 60%. Encaminhe para o programa de Reuso " +
                           $"Doméstico (Second Life) em vez de reciclagem. " +
                           $"Saúde atual: {bateria.SaudeBateria}%."
            });
        }

        // Validar Prioridade
        var prioridadesValidas = new[] { "Baixa", "Alta", "Critica" };
        if (!prioridadesValidas.Contains(ordem.Prioridade))
            return BadRequest(new { mensagem = "Prioridade inválida. Use: Baixa, Alta ou Critica." });

        // Custo de Carbono: taxa extra para estações Ultra-Rapida
        if (estacao.TipoCarga == "Ultra-Rapida")
        {
            ordem.CustoProcessamento += TaxaAmbientalUltraRapida;
            Console.WriteLine($"[GreenDrive] Taxa ambiental de R$ {TaxaAmbientalUltraRapida:F2} aplicada à ordem. " +
                              $"Estação Ultra-Rapida em {estacao.Localizacao}. " +
                              $"Custo total: R$ {ordem.CustoProcessamento:F2}");
        }

        _context.OrdensReciclagem.Add(ordem);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetOrdem), new { id = ordem.Id }, new
        {
            ordem,
            avisoTaxaAplicada = estacao.TipoCarga == "Ultra-Rapida"
                ? $"Taxa ambiental de R$ {TaxaAmbientalUltraRapida:F2} adicionada por uso de estação Ultra-Rapida."
                : null
        });
    }

    // DELETE: api/reciclagem/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrdem(int id)
    {
        var ordem = await _context.OrdensReciclagem.FindAsync(id);
        if (ordem == null)
            return NotFound(new { mensagem = $"Ordem com ID {id} não encontrada." });

        _context.OrdensReciclagem.Remove(ordem);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
