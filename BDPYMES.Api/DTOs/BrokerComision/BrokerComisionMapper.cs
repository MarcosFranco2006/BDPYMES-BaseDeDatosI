using BDPYMES.LogicaServicio.Entidades;

namespace BDPYMES.Api.DTOs;

public static class BrokerComisionMapper
{
    public static BrokerComisionResponse ABrokerComisionResponse(this BrokerComision brokerComision)
    {
        return new BrokerComisionResponse
        {
            IdBroker = brokerComision.IdBroker,
            UltimaActualizacion = brokerComision.UltimaActualizacion,
            PorcentajeComision = brokerComision.PorcentajeComision
        };
    }

    public static BrokerComision ABrokerComision(this BrokerComisionRequest request)
    {
        return new BrokerComision
        {
            IdBroker = request.IdBroker,
            PorcentajeComision = request.PorcentajeComision
        };
    }
}
