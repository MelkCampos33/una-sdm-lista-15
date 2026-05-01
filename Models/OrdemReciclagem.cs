namespace GreenDriveApi20260101.Models;

public class OrdemReciclagem
{
    public int Id { get; set; }
    public int BateriaId { get; set; }
    public int EstacaoId { get; set; }
    public string Prioridade { get; set; } = string.Empty; // Baixa, Alta, Critica
    public decimal CustoProcessamento { get; set; }

    // Navegação
    public Bateria? Bateria { get; set; }
    public EstacaoCarga? Estacao { get; set; }
}
