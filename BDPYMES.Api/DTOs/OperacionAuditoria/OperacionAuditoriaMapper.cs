using BDPYMES.LogicaServicio.Entidades;

namespace BDPYMES.Api.DTOs;

public static class OperacionAuditoriaMapper
{
    public static OperacionAuditoriaResponse AOperacionAuditoriaResponse(this OperacionAuditoria operacionAuditoria)
    {
        return new OperacionAuditoriaResponse
        {
            IdOperacion = operacionAuditoria.IdOperacion,
            FechaOperacion = operacionAuditoria.FechaOperacion,
            MontoOperacion = operacionAuditoria.MontoOperacion,
            TipoAccion = operacionAuditoria.TipoAccion
        };
    }

    public static OperacionAuditoria AOperacionAuditoria(this OperacionAuditoriaRequest request)
    {
        return new OperacionAuditoria
        {
            IdOperacion = request.IdOperacion,
            MontoOperacion = request.MontoOperacion,
            TipoAccion = request.TipoAccion
        };
    }
}
