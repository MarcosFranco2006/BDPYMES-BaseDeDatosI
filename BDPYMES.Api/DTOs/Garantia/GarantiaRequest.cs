using System.ComponentModel.DataAnnotations;

namespace BDPYMES.Api.DTOs;

public class GarantiaRequest
{
    public int CuitPyme {get;set;}
    public string Descripcion {get;set;} = string.Empty;
    public decimal Monto {get;set;}
}
