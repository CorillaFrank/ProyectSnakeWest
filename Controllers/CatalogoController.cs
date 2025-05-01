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
    
    public class CatalogoController : Controller
    {
        private readonly ILogger<CatalogoController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public CatalogoController(ILogger<CatalogoController> logger, ApplicationDbContext context,UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

            public IActionResult Index(string? searchString)
            {
        // Cambia DataProducto por Productos
        var productos = from o in _context.Productos select o;
        
        if(!String.IsNullOrEmpty(searchString))
        {
            productos = productos.Where(s => s.Name.Contains(searchString));
        }
        
        productos = productos.Where(l => l.Status.Contains("A"));
        return View(productos.ToList());
            }

        public async Task<IActionResult> Details(int? id){
            Producto objProduct = await _context.Productos.FindAsync(id);
            if(objProduct == null){
                return NotFound();
            }
            return View(objProduct);
        }
        public async Task<IActionResult> Add(int? id){
             var userID = _userManager.GetUserName(User);
             if(userID==null){
                ViewData["message"] = "Por favor debe logearse antes de agregar un producto";
                List<Producto> productos = new List<Producto>();
                return View("Index",productos); 
              }else{
                var producto = await _context.Productos.FindAsync(id);
                Proforma proforma = new Proforma();
                proforma.Producto = producto;
                proforma.Precio = producto.Price;
                proforma.cantidad =  1;
                proforma.UserID = userID;
                _context.Add(proforma);
                await _context.SaveChangesAsync();
                ViewData["message"] = "Se agrego al carrito";
                return RedirectToAction(nameof(Index));
              }
            
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}