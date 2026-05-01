namespace GreenDriveApi20260101.Models;

public class RegistroTelemetria
{
    public int Id { get; set; }
    public int BateriaId { get; set; }
    public double Temperatura { get; set; }
    public double Voltagem { get; set; }
    public DateTime DataLeitura { get; set; }

    // Navegação
    public Bateria? Bateria { get; set; }
}
