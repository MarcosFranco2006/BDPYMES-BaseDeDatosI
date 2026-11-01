using BDPYMES.LogicaServicio.Entidades;

namespace BDPYMES.Api.DTOs;

public static class GarantiaMapper
{
    public static GarantiaResponse AGarantiaResponse(this Garantia garantia)
    {
        return new GarantiaResponse
        {
            Id = garantia.Id,
            CuitPyme = garantia.CuitPyme,
            Descripcion = garantia.Descripcion,
            Monto = garantia.Monto
        };
    }

    public static Garantia AGarantia(this GarantiaRequest request)
    {
        return new Garantia
        {
            CuitPyme = request.CuitPyme,
            Descripcion = request.Descripcion,
            Monto = request.Monto
        };
    }
}
