using BDPYMES.LogicaServicio.Entidades;

namespace BDPYMES.Api.DTOs;

public static class BrokerMapper
{
    public static BrokerResponse ABrokerResponse(this Broker alyc)
    {
        return new BrokerResponse
        {
            Id = alyc.Id,
            Nombre = alyc.Nombre,
        };
    }

    public static Broker ABroker(this BrokerRequest request)
    {
        return new Broker
        {
            Nombre = request.Nombre,
        };
    }
}
