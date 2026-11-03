namespace BDPYMES.Api.DTOs;

public class OperacionAuditoriaResponse
{
    public int IdOperacion { get; set; }
    public DateTime FechaOperacion { get; set; }
    public decimal MontoOperacion { get; set; }
    public string TipoAccion { get; set; } = string.Empty;
}
