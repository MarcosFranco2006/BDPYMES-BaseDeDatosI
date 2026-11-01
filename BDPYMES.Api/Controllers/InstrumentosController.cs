using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BDPYMES.Api.DTOs;
using BDPYMES.LogicaServicio.Entidades;
using BDPYMES.LogicaServicio.Servicios;

namespace BDPYMES.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InstrumentosController : ControllerBase
{
    private readonly PymesService _servicio;

    public InstrumentosController(PymesService servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public IActionResult ObtenerTodos()
    {
        try{
            IEnumerable<InstrumentoResponse> respuesta = _servicio
                .ConsultarDatosInstrumentos()
                .Select(x => x.AInstrumentoResponse());

            return Ok(respuesta);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Ocurrió un error inesperado", detalle = ex.Message });
        }
    }

    [HttpPost]
    public IActionResult Crear([FromBody] InstrumentoRequest request)
    {
        try
        {
        Instrumento instrumento = request.AInstrumento();

        _servicio.InsertarDatoInstrumento(instrumento);

        return StatusCode(201, instrumento.AInstrumentoResponse());
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
