using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyect_Snake_West.Data;
using Proyect_Snake_West.Models;
using Microsoft.AspNetCore.Authorization;


namespace Proyect_Snake_West.Controllers;

public class PedidoController : Controller
{
    private readonly ApplicationDbContext _db;
    public PedidoController(ApplicationDbContext db) => _db = db;

    // GET /Pedido
    [Authorize(Roles = "Admin")]
    public IActionResult Index()
    {
        var pedidos = _db.Pedidos
                         .AsNoTracking()
                         .OrderByDescending(p => p.ID)
                         .ToList();
        return View(pedidos); // Views/Pedido/Index.cshtml
    }


    // GET /Pedido/Detalle/5
    [Authorize]
    public IActionResult Detalle(int id)
    {
        var pedido = _db.Pedidos
                        .Include(p => p.Detalles)
                        .ThenInclude(d => d.Producto) // opcional
                        .AsNoTracking()
                        .FirstOrDefault(p => p.ID == id);

        if (pedido is null) return NotFound();
        return View(pedido); // Views/Pedido/Detalle.cshtml
    }

    // GET /Pedido/Crear (form)
    public IActionResult Crear() => View(); // Views/Pedido/Crear.cshtml

    // POST /Pedido/Crear (form MVC)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Crear(Pedido pedido)
    {
        if (!ModelState.IsValid) return View(pedido);

        _db.Pedidos.Add(pedido);
        _db.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    // POST /Pedido/CrearJson (via fetch JSON)
    [HttpPost]
    public IActionResult CrearJson([FromBody] Pedido pedido)
    {
        if (pedido == null) return BadRequest();
        _db.Pedidos.Add(pedido);
        _db.SaveChanges();
        return Created($"/Pedido/{pedido.ID}", pedido);
    }
[HttpGet("api/pedidos/status")]
[AllowAnonymous] // opcional, si Flowxo no envía token de autenticación
public IActionResult GetStatus([FromQuery] int orderId)
{
    var pedido = _db.Pedidos.FirstOrDefault(p => p.ID == orderId);

    if (pedido == null)
        return NotFound(new { message = "El pedido no se encuentra registrado, vuelva intentar" });

    // Asegúrate de devolver las propiedades en el mismo formato que espera Flowxo
    return Ok(new
    {
        orderId = pedido.ID,      // con 'I' mayúscula
        user = pedido.UserID,
        total = pedido.Total,
        status = pedido.Status
    });
}

}
