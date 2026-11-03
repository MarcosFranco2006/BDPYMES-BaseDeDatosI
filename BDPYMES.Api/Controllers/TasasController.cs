using Microsoft.AspNetCore.Mvc;
using BDPYMES.Api.DTOs;
using BDPYMES.LogicaServicio.Servicios;

namespace BDPYMES.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasasController : ControllerBase
{
    private readonly PymesService _servicio;

    public TasasController(PymesService servicio)
    {
        _servicio = servicio;
    }

    [HttpPost]
    public IActionResult Registrar([FromBody] TasaRequest request)
    {
        try
        {
            _servicio.RegistrarTasa(request.IdInstrumento, request.TasaInteres);

            return StatusCode(201, new { mensaje = "Tasa registrada correctamente." });
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
