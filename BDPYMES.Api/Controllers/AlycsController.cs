using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BDPYMES.Api.DTOs;
using BDPYMES.LogicaServicio.Entidades;
using BDPYMES.LogicaServicio.Servicios;

namespace BDPYMES.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlycsController : ControllerBase
{
    private readonly PymesService _servicio;

    public AlycsController(PymesService servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public IActionResult ObtenerTodos()
    {
        IEnumerable<AlycResponse> respuesta = _servicio
            .ConsultarDatosAlycs()
            .Select(x => x.AAlycResponse());

        return Ok(respuesta);
    }

    [HttpPost]
    public IActionResult Crear([FromBody] AlycRequest request)
    {
        Alyc alyc = request.AAlyc();

        _servicio.InsertarDatoAlyc(alyc);

        return StatusCode(201, alyc.AAlycResponse());
    }
}
