namespace BDPYMES.Api.DTOs;

public class OperacionAuditoriaRequest
{
    public int IdOperacion { get; set; }
    public decimal MontoOperacion { get; set; }
    public string TipoAccion { get; set; } = string.Empty;
}
