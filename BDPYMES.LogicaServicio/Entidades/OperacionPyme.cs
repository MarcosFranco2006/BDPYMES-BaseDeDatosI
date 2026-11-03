namespace BDPYMES.LogicaServicio.Entidades;

// Una fila de la vista VW_Operaciones_Pymes.
public class OperacionPyme
{
    public int IdOperacion { get; set; }
    public int CuitPyme { get; set; }
    public string RazonSocial { get; set; } = string.Empty;
    public decimal MontoOperacion { get; set; }
    public string Estado { get; set; } = string.Empty;
}
