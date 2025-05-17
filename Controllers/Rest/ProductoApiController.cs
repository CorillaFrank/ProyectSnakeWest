using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Proyect_Snake_West.Data;
using Proyect_Snake_West.Models;
using Proyect_Snake_West.Service;

namespace Proyect_Snake_West.Controllers.Rest
{
    [ApiController]
    [Route("api/producto")]
    public class ProductoApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ProductoService _productoService;

        public ProductoApiController(ApplicationDbContext context, ProductoService productoService)
        {
            _context = context;
            _productoService = productoService;
        }

        // GET: api/producto
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<Producto>>> List()
        {
            var productos = await _productoService.GetAll();
            if (productos == null || productos.Count == 0)
                return NotFound();
            return Ok(productos);
        }

        // GET: api/producto/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Producto>> GetById(int id)
        {
            var producto = await _productoService.GetById(id);
            if (producto == null)
                return NotFound();
            return Ok(producto);
        }

        // POST: api/producto
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Producto>> Create([FromBody] Producto producto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _productoService.Create(producto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/producto/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] Producto producto)
        {
            if (id != producto.Id)
                return BadRequest("El ID de la URL no coincide con el ID del objeto.");

            var updated = await _productoService.Update(id, producto);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/producto/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _productoService.Delete(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
