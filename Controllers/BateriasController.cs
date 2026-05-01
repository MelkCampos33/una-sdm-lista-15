using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GreenDriveApi20260101.Data;
using GreenDriveApi20260101.Models;

namespace GreenDriveApi20260101.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BateriasController : ControllerBase
{
    private readonly AppDbContext _context;

    public BateriasController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/baterias
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Bateria>>> GetBaterias()
    {
        return await _context.Baterias.ToListAsync();
    }

    // GET: api/baterias/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Bateria>> GetBateria(int id)
    {
        var bateria = await _context.Baterias.FindAsync(id);
        if (bateria == null)
            return NotFound(new { mensagem = $"Bateria com ID {id} não encontrada." });

        return bateria;
    }

    // POST: api/baterias
    [HttpPost]
    public async Task<ActionResult<Bateria>> PostBateria(Bateria bateria)
    {
        if (bateria.SaudeBateria < 0 || bateria.SaudeBateria > 100)
            return BadRequest(new { mensagem = "SaudeBateria deve estar entre 0 e 100." });

        _context.Baterias.Add(bateria);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetBateria), new { id = bateria.Id }, bateria);
    }

    // PATCH: api/baterias/5/saude
    /// <summary>
    /// Atualiza a saúde (SoH) de uma bateria.
    /// Regra de Ouro: Se SoH atual &lt;= 10%, a bateria é "Inativa".
    /// Tentar aumentar o SoH de uma bateria inativa retorna 409 Conflict.
    /// </summary>
    [HttpPatch("{id}/saude")]
    public async Task<IActionResult> PatchSaudeBateria(int id, [FromBody] int novaSaude)
    {
        var bateria = await _context.Baterias.FindAsync(id);
        if (bateria == null)
            return NotFound(new { mensagem = $"Bateria com ID {id} não encontrada." });

        if (novaSaude < 0 || novaSaude > 100)
            return BadRequest(new { mensagem = "SaudeBateria deve estar entre 0 e 100." });

        // Regra de Ouro: bateria inativa (SoH <= 10%) não pode ter saúde aumentada
        if (bateria.SaudeBateria <= 10 && novaSaude > bateria.SaudeBateria)
        {
            return Conflict(new
            {
                mensagem = $"CONFLITO DE DADOS: A bateria '{bateria.NumeroSerie}' está INATIVA " +
                           $"(SoH atual: {bateria.SaudeBateria}%). " +
                           $"Tentativa de aumentar para {novaSaude}% foi bloqueada como possível fraude de dados."
            });
        }

        bateria.SaudeBateria = novaSaude;
        await _context.SaveChangesAsync();

        return Ok(new { mensagem = "Saúde da bateria atualizada com sucesso.", saudAtual = bateria.SaudeBateria });
    }

    // DELETE: api/baterias/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBateria(int id)
    {
        var bateria = await _context.Baterias.FindAsync(id);
        if (bateria == null)
            return NotFound(new { mensagem = $"Bateria com ID {id} não encontrada." });

        _context.Baterias.Remove(bateria);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
