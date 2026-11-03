using BDPYMES.LogicaServicio.Entidades;

namespace BDPYMES.Api.DTOs;

public static class GarantiaAlycMapper
{
    public static GarantiaAlycResponse AGarantiaAlycResponse(this GarantiaAlyc garantiaAlyc)
    {
        return new GarantiaAlycResponse
        {
            IdAlyc = garantiaAlyc.IdAlyc,
            IdGarantia = garantiaAlyc.IdGarantia
        };
    }

    public static GarantiaAlyc AGarantiaAlyc(this GarantiaAlycRequest request)
    {
        return new GarantiaAlyc
        {
            IdAlyc = request.IdAlyc,
            IdGarantia = request.IdGarantia
        };
    }
}