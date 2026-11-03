using Microsoft.AspNetCore.Mvc;
using BDPYMES.Api.DTOs;
using BDPYMES.LogicaServicio.Servicios;

namespace BDPYMES.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OperacionesController : ControllerBase
{
    private readonly PymesService _servicio;

    public OperacionesController(PymesService servicio)
    {
        _servicio = servicio;
    }

    // GET api/Operaciones  ->  lee la vista VW_Operaciones_Pymes
    [HttpGet]
    public IActionResult ObtenerTodas()
    {
        try
        {
            IEnumerable<OperacionResponse> respuesta = _servicio
                .ConsultarOperaciones()
                .Select(x => new OperacionResponse
                {
                    IdOperacion = x.IdOperacion,
                    CuitPyme = x.CuitPyme,
                    RazonSocial = x.RazonSocial,
                    MontoOperacion = x.MontoOperacion,
                    Estado = x.Estado
                });

            return Ok(respuesta);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Ocurrió un error inesperado", detalle = ex.Message });
        }
    }
}
