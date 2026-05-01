namespace GreenDriveApi20260101.Models;

public class Bateria
{
    public int Id { get; set; }
    public string NumeroSerie { get; set; } = string.Empty;
    public double CapacidadeKWh { get; set; }
    public int SaudeBateria { get; set; } // 0 a 100 (SoH)

    // Navegação
    public ICollection<RegistroTelemetria> Telemetrias { get; set; } = new List<RegistroTelemetria>();
    public ICollection<OrdemReciclagem> OrdensReciclagem { get; set; } = new List<OrdemReciclagem>();
}
