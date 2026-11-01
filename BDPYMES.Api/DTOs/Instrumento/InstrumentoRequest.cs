using System.ComponentModel.DataAnnotations;

namespace BDPYMES.Api.DTOs;

public class InstrumentoRequest
{
    public string TipoInstrumento {get;set;} = string.Empty;
}
