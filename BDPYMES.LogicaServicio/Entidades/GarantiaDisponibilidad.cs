namespace BDPYMES.LogicaServicio.Entidades;

public class GarantiaDisponibilidad
{
    public int IdGarantia { get; set; }
    public DateTime FechaDesde { get; set; }
    public DateTime FechaHasta { get; set; }
    public int IdOperacion { get; set; }
}
