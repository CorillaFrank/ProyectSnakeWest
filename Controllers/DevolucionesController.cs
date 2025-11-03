using System;
 using System.Collections.Generic;
 using System.Diagnostics;
 using System.Linq;
 using System.Threading.Tasks;
 using Microsoft.AspNetCore.Mvc;
 using Microsoft.Extensions.Logging;
 using Proyect_Snake_West.Data;
 using Microsoft.EntityFrameworkCore;
 using Proyect_Snake_West.Models;
using Microsoft.AspNetCore.Identity;

namespace Proyect_Snake_West.Controllers
{
    public class DevolucionesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public DevolucionesController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public IActionResult RegistrarDevolucion([FromBody] DevolucionRequest request)
        {
            if (string.IsNullOrEmpty(request.Pedido) || string.IsNullOrEmpty(request.Email))
                return BadRequest("Datos incompletos.");

            var devolucion = new Devolucion
            {
                Pedido = request.Pedido,
                Email = request.Email,
                Motivo = request.Motivo,
                FechaSolicitud = DateTime.Now
            };

            _db.Devoluciones.Add(devolucion);
            _db.SaveChanges();

            return Ok(new { Message = "Solicitud registrada correctamente.", DevolucionId = devolucion.Id });

        }
    }

    public class DevolucionRequest
    {
        public string Pedido { get; set; }
        public string Email { get; set; }
        public string Motivo { get; set; }
    }
}