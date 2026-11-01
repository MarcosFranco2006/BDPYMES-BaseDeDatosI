using BDPYMES.LogicaServicio.Entidades;

namespace BDPYMES.Api.DTOs;

public static class PymeMapper
{
    public static PymeResponse APymeResponse(this Pyme pyme)
    {
        return new PymeResponse
        {
            CUIT = pyme.CUIT,
            Direccion = pyme.Direccion,
            Razon_Social = pyme.Razon_Social
        };
    }

    public static Pyme APyme(this PymeRequest request)
    {
        return new Pyme
        {
            CUIT = request.CUIT,
            Direccion = request.Direccion,
            Razon_Social = request.Razon_Social
        };
    }
}
