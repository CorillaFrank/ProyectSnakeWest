using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Proyect_Snake_West.Models;
using Proyect_Snake_West.Data;

namespace Proyect_Snake_West.Service
{
    public class ProductoService
    {
        private readonly ILogger<ProductoService> _logger;
        private readonly ApplicationDbContext _context;

        public ProductoService(ILogger<ProductoService> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: Listar todos los productos
        public async Task<List<Producto>?> GetAll()
        {
            if (_context.Productos == null)
                return null;

            return await _context.Productos.ToListAsync();
        }

        // GET: Obtener producto por ID
        public async Task<Producto?> GetById(int id)
        {
            return await _context.Productos.FindAsync(id);
        }

        // POST: Crear un nuevo producto
        public async Task<Producto?> Create(Producto producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
            return producto;
        }

        // PUT: Actualizar un producto existente
        public async Task<bool> Update(int id, Producto producto)
        {
            var existing = await _context.Productos.FindAsync(id);
            if (existing == null)
                return false;

            existing.Name = producto.Name;
            existing.Price = producto.Price;
            existing.Status = producto.Status;
            existing.ImageURL = producto.ImageURL;

            await _context.SaveChangesAsync();
            return true;
        }

        // DELETE: Eliminar un producto por ID
        public async Task<bool> Delete(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
                return false;

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
