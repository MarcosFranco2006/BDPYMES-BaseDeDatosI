using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BDPYMES.Api.DTOs;
using BDPYMES.LogicaServicio.Entidades;
using BDPYMES.LogicaServicio.Servicios;

namespace BDPYMES.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PymesController : ControllerBase
{
    private readonly PymesService _servicio;

    public PymesController(PymesService servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public IActionResult ObtenerTodos()
    {
        IEnumerable<PymeResponse> respuesta = _servicio
            .ConsultarDatosPymes()
            .Select(x => x.APymeResponse());

        return Ok(respuesta);
    }

    [HttpPost]
    public IActionResult Crear([FromBody] PymeRequest request)
    {
        Pyme pyme = request.APyme();

        _servicio.InsertarDatoPyme(pyme);

        return StatusCode(201, pyme.APymeResponse());
    }
}
