using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BDPYMES.Api.DTOs;
using BDPYMES.LogicaServicio.Entidades;
using BDPYMES.LogicaServicio.Servicios;

namespace BDPYMES.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrokersController : ControllerBase
{
    private readonly PymesService _servicio;

    public BrokersController(PymesService servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public IActionResult ObtenerTodos()
    {
        IEnumerable<BrokerResponse> respuesta = _servicio
            .ConsultarDatosBrokers()
            .Select(x => x.ABrokerResponse());

        return Ok(respuesta);
    }

    [HttpPost]
    public IActionResult Crear([FromBody] BrokerRequest request)
    {
        Broker broker = request.ABroker();

        _servicio.InsertarDatoBroker(broker);

        return StatusCode(201, broker.ABrokerResponse());
    }
}
