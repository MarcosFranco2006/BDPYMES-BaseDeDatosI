using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BDPYMES.Api.DTOs;
using BDPYMES.LogicaServicio.Entidades;
using BDPYMES.LogicaServicio.Servicios;

namespace BDPYMES.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GarantiasAlycController : ControllerBase
{
    private readonly PymesService _servicio;

    public GarantiasAlycController(PymesService servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public IActionResult ObtenerTodos()
    {
        try
        {
            IEnumerable<GarantiaAlycResponse> respuesta = _servicio
                .ConsultarDatosGarantiasAlycs()
                .Select(x => x.AGarantiaAlycResponse());

            return Ok(respuesta);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Ocurrió un error inesperado", detalle = ex.Message });
        }
    }

    [HttpPost]
    public IActionResult Crear([FromBody] GarantiaAlycRequest request)
    {
        try
        {
            GarantiaAlyc disponibilidad = request.AGarantiaAlyc();

            _servicio.InsertarDatoGarantiaAlyc(disponibilidad);

            return StatusCode(201, disponibilidad.AGarantiaAlycResponse());
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
