using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyect_Snake_West.Data;
using Proyect_Snake_West.Models;
using System.ComponentModel.DataAnnotations;
using System.Text; // <- lo estabas usando sin importar

namespace Proyect_Snake_West.Controllers
{
    [ApiController]
    [Route("api/devoluciones")]
    public class DevolucionesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public DevolucionesController(ApplicationDbContext db) => _db = db;

        // DTO de entrada con validación
        public sealed class DevolucionRequest
        {
            [Required] public string Pedido { get; set; } = default!;
            [Required, EmailAddress] public string Email { get; set; } = default!;
            public string? Motivo { get; set; }
            // si tu DB NO permite null aquí, hazlo int (no nullable) y quita el default 0 al crear
            public int? ProductoId { get; set; }
        }

        // POST api/devoluciones
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegistrarDevolucion([FromBody] DevolucionRequest request)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            // validar existencia de producto si viene
            if (request.ProductoId is int pid)
            {
                var existe = await _db.Productos.AnyAsync(p => p.Id == pid);
                if (!existe) return BadRequest(new { message = "El producto no existe." });
            }

            var devolucion = new Devolucion
            {
                Pedido = request.Pedido,
                Email = request.Email,
                Motivo = request.Motivo ?? "",
                FechaSolicitud = DateTime.UtcNow,
                ProductoId = request.ProductoId ?? 0 // ajusta si la columna no acepta 0
            };

            _db.Devoluciones.Add(devolucion);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(ObtenerPorId), new { id = devolucion.Id }, new
            {
                devolucion.Id,
                devolucion.Pedido,
                devolucion.Email,
                devolucion.Motivo,
                devolucion.FechaSolicitud,
                devolucion.ProductoId
            });
        }

        // GET api/devoluciones/{id}
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var d = await _db.Devoluciones
                .Include(x => x.Producto)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (d is null) return NotFound(new { message = "Devolución no encontrada." });

            return Ok(new
            {
                d.Id,
                d.Pedido,
                d.Email,
                d.Motivo,
                d.FechaSolicitud,
                d.ProductoId,
                Producto = d.Producto == null
                    ? null
                    : new { d.Producto.Id, d.Producto.Name, d.Producto.Price, d.Producto.ImageURL }
            });
        }

        // GET api/devoluciones
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var list = await _db.Devoluciones
                .AsNoTracking()
                .OrderByDescending(x => x.FechaSolicitud)
                .Select(d => new { d.Id, d.Pedido, d.Email, d.Motivo, d.FechaSolicitud, d.ProductoId })
                .ToListAsync();

            return Ok(list);
        }

        // POST api/devoluciones/registrarTexto  (respuesta en text/plain para Flow XO)
        [HttpPost("registrarTexto")]
        [Produces("text/plain")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RegistrarTexto([FromBody] DevolucionRequest r)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos incompletos o inválidos.");

            // validar pedido
            var pedido = await _db.Pedidos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ID.ToString() == r.Pedido);
            if (pedido is null)
                return NotFound("No pudimos validar el pedido. Verifica el número e inténtalo nuevamente.");

            // validar producto
            var existeProducto = await _db.Productos.AnyAsync(p => p.Id == (r.ProductoId ?? 0));
            if (!existeProducto)
                return BadRequest("El producto no existe o no coincide con el pedido.");

            var devolucion = new Devolucion
            {
                Pedido = r.Pedido,
                Email = r.Email,
                Motivo = r.Motivo ?? "Sin motivo",
                FechaSolicitud = DateTime.UtcNow,
                ProductoId = r.ProductoId ?? 0
            };

            _db.Devoluciones.Add(devolucion);
            await _db.SaveChangesAsync();

            var sb = new StringBuilder();
            sb.AppendLine("✅ Devolución registrada correctamente.");
            sb.AppendLine($"Pedido: {devolucion.Pedido}");
            sb.AppendLine($"Motivo: {devolucion.Motivo}");
            sb.AppendLine($"Producto ID: {devolucion.ProductoId}");
            sb.AppendLine($"Fecha: {devolucion.FechaSolicitud:dd/MM/yyyy HH:mm}");
            sb.AppendLine();
            sb.AppendLine($"Recibirás un correo de confirmación en las próximas horas al siguiente  {r.Email}.");

            return Content(sb.ToString(), "text/plain", Encoding.UTF8);
        }
    }
}
