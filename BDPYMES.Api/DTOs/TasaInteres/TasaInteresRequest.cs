using System.ComponentModel.DataAnnotations;

namespace BDPYMES.Api.DTOs;

public class TasaInteresRequest
{
    public int IdInstrumento {get;set;}
    public decimal TasaDeInteres {get;set;}
}
