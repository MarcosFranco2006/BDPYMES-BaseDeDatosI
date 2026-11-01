using System.ComponentModel.DataAnnotations;

namespace BDPYMES.Api.DTOs;

public class BrokerComisionRequest
{
    public int IdBroker {get;set;}
    public decimal PorcentajeComision {get;set;}
}
