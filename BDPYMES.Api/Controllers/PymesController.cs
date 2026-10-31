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

    // El servicio llega por inyección de dependencias: está registrado en Program.cs.
    public PymesController(PymesService servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public IActionResult ObtenerTodos()
    {
        IEnumerable<PymeResponse> respuesta = _servicio
            .ConsultarDatos(_servicio.connectionString)
            .Select(producto => producto.APymeResponse());

        return Ok(respuesta);
    }

    // POST api/producto
    [HttpPost]
    public IActionResult Crear([FromBody] PymeRequest request)
    {
        Pyme pyme = request.APyme();

        _servicio.InsertarDato(_servicio.connectionString, pyme);

        return StatusCode(201, pyme.APymeResponse());
    }
}
