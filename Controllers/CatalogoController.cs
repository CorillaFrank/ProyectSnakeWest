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

namespace Proyect_Snake_West.Controllers
{
    
    public class CatalogoController : Controller
    {
        private readonly ILogger<CatalogoController> _logger;
        private readonly ApplicationDbContext _context;
        public CatalogoController(ILogger<CatalogoController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}