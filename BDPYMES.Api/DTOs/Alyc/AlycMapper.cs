using BDPYMES.LogicaServicio.Entidades;

namespace BDPYMES.Api.DTOs;

public static class AlycMapper
{
    public static AlycResponse AAlycResponse(this Alyc alyc)
    {
        return new AlycResponse
        {
            Id = alyc.Id,
            Nombre = alyc.Nombre,
        };
    }

    public static Alyc AAlyc(this AlycRequest request)
    {
        return new Alyc
        {
            Nombre = request.Nombre,
        };
    }
}
