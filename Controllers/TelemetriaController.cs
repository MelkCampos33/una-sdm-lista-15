using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GreenDriveApi20260101.Data;
using GreenDriveApi20260101.Models;

namespace GreenDriveApi20260101.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TelemetriaController : ControllerBase
{
    private readonly AppDbContext _context;

    public TelemetriaController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RegistroTelemetria>>> GetTelemetrias()
    {
        return await _context.RegistrosTelemetria
            .Include(t => t.Bateria)
            .ToListAsync();
    }

    [HttpGet("bateria/{bateriaId}")]
    public async Task<ActionResult<IEnumerable<RegistroTelemetria>>> GetTelemetriaPorBateria(int bateriaId)
    {
        var existe = await _context.Baterias.AnyAsync(b => b.Id == bateriaId);
        if (!existe)
            return NotFound(new { mensagem = $"Bateria com ID {bateriaId} não encontrada." });

        var telemetrias = await _context.RegistrosTelemetria
            .Where(t => t.BateriaId == bateriaId)
            .ToListAsync();

        return telemetrias;
    }

   
    [HttpPost]
    public async Task<ActionResult<RegistroTelemetria>> PostTelemetria(RegistroTelemetria telemetria)
    {
        var bateria = await _context.Baterias.FindAsync(telemetria.BateriaId);
        if (bateria == null)
            return NotFound(new { mensagem = $"Bateria com ID {telemetria.BateriaId} não encontrada. Integridade violada." });

        if (telemetria.Temperatura > 85)
        {
            Console.WriteLine($" ALERTA DE SEGURANÇA: Risco térmico detectado na bateria {bateria.NumeroSerie}! Registro bloqueado para investigação.");
            return BadRequest(new
            {
                mensagem = $"ALERTA DE SEGURANÇA: Risco térmico detectado na bateria {bateria.NumeroSerie}! " +
                           $"Registro bloqueado para investigação. Temperatura informada: {telemetria.Temperatura}°C (Limite: 85°C)."
            });
        }

        telemetria.DataLeitura = telemetria.DataLeitura == default
            ? DateTime.UtcNow
            : telemetria.DataLeitura;

        _context.RegistrosTelemetria.Add(telemetria);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTelemetrias), new { id = telemetria.Id }, telemetria);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTelemetria(int id)
    {
        var telemetria = await _context.RegistrosTelemetria.FindAsync(id);
        if (telemetria == null)
            return NotFound(new { mensagem = $"Registro de telemetria com ID {id} não encontrado." });

        _context.RegistrosTelemetria.Remove(telemetria);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
