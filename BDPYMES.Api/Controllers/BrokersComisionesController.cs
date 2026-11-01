using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BDPYMES.Api.DTOs;
using BDPYMES.LogicaServicio.Entidades;
using BDPYMES.LogicaServicio.Servicios;

namespace BDPYMES.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrokersComisionesController : ControllerBase
{
    private readonly PymesService _servicio;

    public BrokersComisionesController(PymesService servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public IActionResult ObtenerTodos()
    {
        try{
            IEnumerable<BrokerComisionResponse> respuesta = _servicio
                .ConsultarDatosBrokerComisiones()
                .Select(x => x.ABrokerComisionResponse());

            return Ok(respuesta);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Ocurrió un error inesperado", detalle = ex.Message });
        }
    }

    [HttpPost]
    public IActionResult Crear([FromBody] BrokerComisionRequest request)
    {
        try
        {
        BrokerComision brokerComision = request.ABrokerComision();

        _servicio.InsertarDatoBrokerComision(brokerComision);

        return StatusCode(201, brokerComision.ABrokerComisionResponse());
        }        
        catch (ArgumentException ex)
        {
            return StatusCode(400, new { mensaje = "Dato mal brindado", detalle = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Ocurrió un error inesperado", detalle = ex.Message });
        }
    }
}
