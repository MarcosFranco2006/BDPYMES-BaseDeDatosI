namespace BDPYMES.LogicaServicio.Entidades;

public class Garantia
{
    public int Id {get;set;}
    public string CuitPyme {get;set;} = string.Empty;
    public string Descripcion {get;set;} = string.Empty;
    public decimal Monto {get;set;}
}
