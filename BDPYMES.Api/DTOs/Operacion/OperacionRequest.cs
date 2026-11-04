namespace BDPYMES.Api.DTOs.Operacion;

public class OperacionRequest
{
    public int CuitPyme { get; set; }
    public int IdAlyc { get; set; }
    public int IdBroker { get; set; }
    public int IdInstrumento { get; set; }
    public int IdSgr { get; set; }
    public string Estado { get; set; } = string.Empty;
    public decimal MontoOperacion { get; set; }
}