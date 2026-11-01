using BDPYMES.LogicaServicio.Entidades;

namespace BDPYMES.Api.DTOs;

public static class SgrMapper
{
    public static SgrResponse ASgrResponse(this Sgr sgr)
    {
        return new SgrResponse
        {
            Id = sgr.Id,
            Nombre = sgr.Nombre,
        };
    }

    public static Sgr ASgr(this SgrRequest request)
    {
        return new Sgr
        {
            Nombre = request.Nombre,
        };
    }
}
