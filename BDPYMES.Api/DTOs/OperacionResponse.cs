namespace BDPYMES.Api.DTOs;

public class OperacionResponse
{
    public int IdOperacion { get; set; }
    public int CuitPyme { get; set; }
    public string RazonSocial { get; set; } = string.Empty;
    public decimal MontoOperacion { get; set; }
    public string Estado { get; set; } = string.Empty;
}
