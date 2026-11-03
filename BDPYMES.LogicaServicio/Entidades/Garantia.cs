namespace BDPYMES.LogicaServicio.Entidades;

public class Garantia
{
    public int Id {get;set;}
    public int CuitPyme {get;set;}
    public string Descripcion {get;set;} = string.Empty;
    public decimal Monto {get;set;}
}
