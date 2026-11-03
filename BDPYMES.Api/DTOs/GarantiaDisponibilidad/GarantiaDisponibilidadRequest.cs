namespace BDPYMES.Api.DTOs;

public class GarantiaDisponibilidadRequest
{
    public int IdGarantia { get; set; }
    public DateTime FechaDesde { get; set; }
    public DateTime FechaHasta { get; set; }
    public int IdOperacion { get; set; }
}
