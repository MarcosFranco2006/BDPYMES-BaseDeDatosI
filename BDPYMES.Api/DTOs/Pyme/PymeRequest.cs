using System.ComponentModel.DataAnnotations;

namespace BDPYMES.Api.DTOs;

public class PymeRequest
{
    public int CUIT {get;set;}
    public string Direccion {get;set;} = string.Empty;
    public string Razon_Social {get;set;} = string.Empty;
}
