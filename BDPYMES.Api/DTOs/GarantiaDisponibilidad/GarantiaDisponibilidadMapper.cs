using BDPYMES.LogicaServicio.Entidades;

namespace BDPYMES.Api.DTOs;

public static class GarantiaDisponibilidadMapper
{
    public static GarantiaDisponibilidadResponse AGarantiaDisponibilidadResponse(this GarantiaDisponibilidad disponibilidad)
    {
        return new GarantiaDisponibilidadResponse
        {
            IdGarantia = disponibilidad.IdGarantia,
            FechaDesde = disponibilidad.FechaDesde,
            FechaHasta = disponibilidad.FechaHasta,
            IdOperacion = disponibilidad.IdOperacion
        };
    }

    public static GarantiaDisponibilidad AGarantiaDisponibilidad(this GarantiaDisponibilidadRequest request)
    {
        return new GarantiaDisponibilidad
        {
            IdGarantia = request.IdGarantia,
            FechaDesde = request.FechaDesde,
            FechaHasta = request.FechaHasta,
            IdOperacion = request.IdOperacion
        };
    }
}
