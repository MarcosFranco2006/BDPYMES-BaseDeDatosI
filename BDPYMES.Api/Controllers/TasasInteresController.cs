using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BDPYMES.Api.DTOs;
using BDPYMES.LogicaServicio.Entidades;
using BDPYMES.LogicaServicio.Servicios;

namespace BDPYMES.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasasInteresController : ControllerBase
{
    private readonly PymesService _servicio;

    public TasasInteresController(PymesService servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public IActionResult ObtenerTodos()
    {
        try{
            IEnumerable<TasaInteresResponse> respuesta = _servicio
                .ConsultarDatosTasasInteres()
                .Select(x => x.ATasaInteresResponse());

            return Ok(respuesta);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Ocurrió un error inesperado", detalle = ex.Message });
        }
    }

    [HttpPost]
    public IActionResult Crear([FromBody] TasaInteresRequest request)
    {
        try
        {
        TasaInteres tasaInteres = request.ATasaInteres();

        _servicio.InsertarDatoTasaInteres(tasaInteres);

        return StatusCode(201, tasaInteres.ATasaInteresResponse());
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
