namespace GreenDriveApi20260101.Models;

public class EstacaoCarga
{
    public int Id { get; set; }
    public string Localizacao { get; set; } = string.Empty; // Cidade/UF
    public string TipoCarga { get; set; } = string.Empty; // Rapida, Ultra-Rapida, Residencial
    public double CargaDisponivelKW { get; set; }

    // Navegação
    public ICollection<OrdemReciclagem> OrdensReciclagem { get; set; } = new List<OrdemReciclagem>();
}
