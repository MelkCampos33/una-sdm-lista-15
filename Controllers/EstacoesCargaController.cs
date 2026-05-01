using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GreenDriveApi20260101.Data;
using GreenDriveApi20260101.Models;

namespace GreenDriveApi20260101.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstacoesCargaController : ControllerBase
{
    private readonly AppDbContext _context;

    public EstacoesCargaController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/estacoescarga
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EstacaoCarga>>> GetEstacoes()
    {
        return await _context.EstacoesCarga.ToListAsync();
    }

    // GET: api/estacoescarga/5
    [HttpGet("{id}")]
    public async Task<ActionResult<EstacaoCarga>> GetEstacao(int id)
    {
        var estacao = await _context.EstacoesCarga.FindAsync(id);
        if (estacao == null)
            return NotFound(new { mensagem = $"Estação com ID {id} não encontrada." });

        return estacao;
    }

    // POST: api/estacoescarga
    [HttpPost]
    public async Task<ActionResult<EstacaoCarga>> PostEstacao(EstacaoCarga estacao)
    {
        var tiposValidos = new[] { "Rapida", "Ultra-Rapida", "Residencial" };
        if (!tiposValidos.Contains(estacao.TipoCarga))
            return BadRequest(new { mensagem = "TipoCarga inválido. Use: Rapida, Ultra-Rapida ou Residencial." });

        _context.EstacoesCarga.Add(estacao);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetEstacao), new { id = estacao.Id }, estacao);
    }

    // PUT: api/estacoescarga/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutEstacao(int id, EstacaoCarga estacao)
    {
        if (id != estacao.Id)
            return BadRequest(new { mensagem = "ID da rota não corresponde ao ID da entidade." });

        _context.Entry(estacao).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/estacoescarga/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEstacao(int id)
    {
        var estacao = await _context.EstacoesCarga.FindAsync(id);
        if (estacao == null)
            return NotFound(new { mensagem = $"Estação com ID {id} não encontrada." });

        _context.EstacoesCarga.Remove(estacao);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
