using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Proyect_Snake_West.Models;
using Proyect_Snake_West.Data;

namespace Proyect_Snake_West.Service
{
    public class ProductoService
    {
        private readonly ILogger<ProductoService> _logger;
        private readonly ApplicationDbContext _context;

         public ProductoService(ILogger<ProductoService> logger,ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<List<Producto>?> GetAll(){
            if(_context.Productos == null )
                return null;
            return await _context.Productos.ToListAsync();
        }
    }
}