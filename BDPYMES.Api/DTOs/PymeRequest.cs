using System.ComponentModel.DataAnnotations;

namespace BDPYMES.Api.DTOs;

public class PymeRequest
{
    public string CUIT {get;set;} = string.Empty;
    public string Direccion {get;set;} = string.Empty;
    public string Razon_Social {get;set;} = string.Empty;
}
