namespace BDPYMES.LogicaServicio.Entidades;

public class Operacion
{
    public int Id { get; set; }
    public int CuitPyme { get; set; }
    public int IdAlyc { get; set; }
    public int IdBroker { get; set; }
    public int IdInstrumento { get; set; }
    public int IdSgr { get; set; }
    public string Estado { get; set; } = string.Empty;
    public decimal MontoOperacion { get; set; }
}