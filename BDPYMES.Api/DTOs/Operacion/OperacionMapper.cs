using OperacionEntidad = BDPYMES.LogicaServicio.Entidades.Operacion;

namespace BDPYMES.Api.DTOs.Operacion;

public static class OperacionMapper
{
    public static OperacionResponse AOperacionResponse(this OperacionEntidad operacion)
    {
        return new OperacionResponse
        {
            IdOperacion = operacion.Id,
            CuitPyme = operacion.CuitPyme,
            IdAlyc = operacion.IdAlyc,
            IdBroker = operacion.IdBroker,
            IdInstrumento = operacion.IdInstrumento,
            IdSgr = operacion.IdSgr,
            Estado = operacion.Estado,
            MontoOperacion = operacion.MontoOperacion
        };
    }

    public static OperacionEntidad AOperacion(this OperacionRequest request)
    {
        return new OperacionEntidad
        {
            CuitPyme = request.CuitPyme,
            IdAlyc = request.IdAlyc,
            IdBroker = request.IdBroker,
            IdInstrumento = request.IdInstrumento,
            IdSgr = request.IdSgr,
            Estado = request.Estado,
            MontoOperacion = request.MontoOperacion
        };
    }
}