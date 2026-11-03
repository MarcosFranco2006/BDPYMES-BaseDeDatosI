namespace BDPYMES.Api.DTOs;

public class GarantiaDisponibilidadResponse
{
    public int IdGarantia { get; set; }
    public DateTime FechaDesde { get; set; }
    public DateTime FechaHasta { get; set; }
    public int IdOperacion { get; set; }
}
