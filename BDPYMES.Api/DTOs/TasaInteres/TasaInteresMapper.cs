using BDPYMES.LogicaServicio.Entidades;

namespace BDPYMES.Api.DTOs;

public static class TasaInteresMapper
{
    public static TasaInteresResponse ATasaInteresResponse(this TasaInteres tasaInteres)
    {
        return new TasaInteresResponse
        {
            IdInstrumento = tasaInteres.IdInstrumento,
            UltimaActualizacion = tasaInteres.UltimaActualizacion,
            TasaDeInteres = tasaInteres.TasaDeInteres
        };
    }

    public static TasaInteres ATasaInteres(this TasaInteresRequest request)
    {
        return new TasaInteres
        {
            IdInstrumento = request.IdInstrumento,
            TasaDeInteres = request.TasaDeInteres
        };
    }
}
