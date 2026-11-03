using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BDPYMES.Api.DTOs;
using BDPYMES.LogicaServicio.Entidades;
using BDPYMES.LogicaServicio.Servicios;

namespace BDPYMES.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GarantiasDisponibilidadController : ControllerBase
{
    private readonly PymesService _servicio;

    public GarantiasDisponibilidadController(PymesService servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public IActionResult ObtenerTodos()
    {
        try
        {
            IEnumerable<GarantiaDisponibilidadResponse> respuesta = _servicio
                .ConsultarDatosGarantiasDisponibilidad()
                .Select(x => x.AGarantiaDisponibilidadResponse());

            return Ok(respuesta);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Ocurrió un error inesperado", detalle = ex.Message });
        }
    }

    [HttpPost]
    public IActionResult Crear([FromBody] GarantiaDisponibilidadRequest request)
    {
        try
        {
            GarantiaDisponibilidad disponibilidad = request.AGarantiaDisponibilidad();

            _servicio.InsertarDatoGarantiaDisponibilidad(disponibilidad);

            return Created("", disponibilidad.AGarantiaDisponibilidadResponse());        }
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
