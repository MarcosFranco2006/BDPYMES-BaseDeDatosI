using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BDPYMES.Api.DTOs;
using BDPYMES.LogicaServicio.Entidades;
using BDPYMES.LogicaServicio.Servicios;

namespace BDPYMES.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OperacionesAuditoriaController : ControllerBase
{
    private readonly PymesService _servicio;

    public OperacionesAuditoriaController(PymesService servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public IActionResult ObtenerTodos()
    {
        try
        {
            IEnumerable<OperacionAuditoriaResponse> respuesta = _servicio
                .ConsultarDatosOperacionesAuditoria()
                .Select(x => x.AOperacionAuditoriaResponse());

            return Ok(respuesta);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Ocurrió un error inesperado", detalle = ex.Message });
        }
    }

    [HttpPost]
    public IActionResult Crear([FromBody] OperacionAuditoriaRequest request)
    {
        try
        {
            OperacionAuditoria operacionAuditoria = request.AOperacionAuditoria();

            OperacionAuditoria creada = _servicio.InsertarDatoOperacionAuditoria(operacionAuditoria);

            return StatusCode(201, creada.AOperacionAuditoriaResponse());
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
