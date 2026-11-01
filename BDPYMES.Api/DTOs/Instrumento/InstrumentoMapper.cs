using BDPYMES.LogicaServicio.Entidades;

namespace BDPYMES.Api.DTOs;

public static class InstrumentoMapper
{
    public static InstrumentoResponse AInstrumentoResponse(this Instrumento instrumento)
    {
        return new InstrumentoResponse
        {
            Id = instrumento.Id,
            TipoInstrumento = instrumento.TipoInstrumento,
        };
    }

    public static Instrumento AInstrumento(this InstrumentoRequest request)
    {
        return new Instrumento
        {
            TipoInstrumento = request.TipoInstrumento,
        };
    }
}
