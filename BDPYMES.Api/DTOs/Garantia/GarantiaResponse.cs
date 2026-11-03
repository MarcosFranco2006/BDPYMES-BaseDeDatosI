namespace BDPYMES.Api.DTOs;

public class GarantiaResponse
{
    public int Id {get;set;}
    public int CuitPyme {get;set;}
    public string Descripcion {get;set;} = string.Empty;
    public decimal Monto {get;set;}
}
