using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Proyect_Snake_West.Models;
using Proyect_Snake_West.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;



namespace Proyect_Snake_West.Controllers
{
   
    public class CarritoController : Controller
    {
        private readonly ILogger<CarritoController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public CarritoController(ILogger<CarritoController> logger,
         ApplicationDbContext context,
         UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
             _context = context;
        }


        
        public IActionResult IndexUltimoProductoSesion()
        {
            var producto = Util.SessionExtensions.Get<Producto>(HttpContext.Session,"MiUltimoProducto");
            return View("UltimoProducto", producto);
        }
        public IActionResult Index()
        {
            var userIDSession = _userManager.GetUserName(User);
            if(userIDSession == null){
                ViewData["Message"] = "Por favor debe loguearse antes de agregar un producto";
                return RedirectToAction("Index","Catalogo");
            }
            var items = from o in _context.Carritos select o;
            items = items.Include(p => p.Producto).
                    Where(w => w.UserID.Equals(userIDSession) &&
                        w.Status.Equals("PENDIENTE"));
            var itemsCarrito = items.ToList();
            var total = itemsCarrito.Sum(c => c.Cantidad * c.Precio);

            dynamic model = new ExpandoObject();
            model.montoTotal = total;
            model.elementosCarrito = itemsCarrito;
            return View(model);
        }
         public async Task<IActionResult> Add(int? id){
            var userID = _userManager.GetUserName(User);
            if(userID == null){
                _logger.LogInformation("No existe usuario");
                ViewData["Message"] = "Por favor debe loguearse antes de agregar un producto";
                return RedirectToAction("Index","Catalogo");
            }else{
                var producto = await _context.Productos.FindAsync(id);
                Util.SessionExtensions.Set<Producto>(HttpContext.Session,"MiUltimoProducto", producto);
                Proforma proforma = new Proforma();
                proforma.Producto = producto;
                proforma.Precio = producto.Price;
                proforma.Cantidad = 1;
                proforma.UserID = userID;
                _context.Add(proforma);
                await _context.SaveChangesAsync();
                ViewData["Message"] = "Se Agrego al carrito";
                _logger.LogInformation("Se agrego un producto al carrito");
                return RedirectToAction("Index","Catalogo");
            }
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemCarrito = await _context.Carritos.FindAsync(id);
            _context.Carritos.Remove(itemCarrito);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemCarrito = await _context.Carritos.FindAsync(id);
            if (itemCarrito == null)
            {
                return NotFound();
            }
            return View(itemCarrito);
        }

         [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Cantidad,Precio,UserID")] Proforma itemCarrito)
        {
            if (id != itemCarrito.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(itemCarrito);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Carritos.Any(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(itemCarrito);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}