using System.ComponentModel.DataAnnotations;

namespace BDPYMES.Api.DTOs;

public class GarantiaRequest
{
    public string CuitPyme {get;set;} = string.Empty;
    public string Descripcion {get;set;} = string.Empty;
    public decimal Monto {get;set;}
}
