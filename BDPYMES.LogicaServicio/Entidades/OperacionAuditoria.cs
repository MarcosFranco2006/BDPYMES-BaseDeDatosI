namespace BDPYMES.LogicaServicio.Entidades;

public class OperacionAuditoria
{
    public int IdOperacion { get; set; }
    public DateTime FechaOperacion { get; set; }
    public decimal MontoOperacion { get; set; }
    public string TipoAccion { get; set; } = string.Empty;
}
