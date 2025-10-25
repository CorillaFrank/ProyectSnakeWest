using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyect_Snake_West.Data;
using Proyect_Snake_West.Models;

namespace Proyect_Snake_West.Controllers
{
    [Authorize] // exige login para todo el flujo de pago
    public class PagoController : Controller
    {
        private readonly IHttpClientFactory _http;
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _db;

        public PagoController(IHttpClientFactory http, IConfiguration config, ApplicationDbContext db)
        {
            _http = http;
            _config = config;
            _db = db;
        }

        // GET /Pago/Create?montoTotal=123.45
        [HttpGet]
        public IActionResult Create(decimal montoTotal = 0)
        {
            var model = new Pago
            {
                UserID = User?.Identity?.Name ?? "Invitado",
                MontoTotal = montoTotal,
                PaymentDate = DateTime.UtcNow
            };
            return View(model);
        }

        // POST /Pago/PayWithPayPal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayWithPayPal(decimal monto)
        {
            var baseUrl = _config["Node:BaseUrl"] ?? "http://localhost:3000";
            var cli = _http.CreateClient();

            var montoStr = monto.ToString("0.00", CultureInfo.InvariantCulture);
            var url = $"{baseUrl}/create-paypal-order?monto={montoStr}";

            var resp = await cli.GetAsync(url);
            var body = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                return Content($"Node devolvió {resp.StatusCode}:\n{body}", "text/plain");
            }

            var order = JsonSerializer.Deserialize<PayPalOrderResponse>(
                body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var approve = order?.Links?.FirstOrDefault(l => l.Rel == "approve")?.Href;
            if (string.IsNullOrEmpty(approve))
                return Content($"Orden sin link de aprobación:\n{body}", "text/plain");

            return Redirect(approve);
        }

        // GET /Pago/ConfirmarPago?token=EC-XXXX
        [HttpGet]
        public async Task<IActionResult> ConfirmarPago(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest("Token inválido.");

            // 1) Completar/capturar la orden en tu servicio Node
            var baseUrl = _config["Node:BaseUrl"] ?? "http://localhost:3000";
            var cli = _http.CreateClient();
            var resp = await cli.GetAsync($"{baseUrl}/complete-order?token={token}");
            if (!resp.IsSuccessStatusCode)
                return View("Cancelacion");

            // 2) Usuario actual (ajusta si usas el Id de Identity en vez de Name)
            var userId = User?.Identity?.Name ?? "Invitado";

            // 3) Traer el carrito del usuario
            var itemsCarrito = await _db.Carritos
                .Include(c => c.Producto)           // necesitamos Producto.Id
                .Where(c => c.UserID == userId)     // <-- si no tienes UserID en Proforma, cambia este filtro por tu criterio
                .ToListAsync();

            // Si no manejas UserID en el carrito, puedes omitir detalles o usar tu propia lógica
            // if (!itemsCarrito.Any()) return View("Confirmacion");

            // 4) Calcular total (o cruzar con el monto retornado por Node/PayPal)
            var total = itemsCarrito.Sum(i => i.Precio * i.Cantidad);

            // 5) Crear Pedido
            var pedido = new Pedido
            {
                UserID = userId,
                Total = total,
                Status = "Pagado"
            };
            _db.Pedidos.Add(pedido);
            await _db.SaveChangesAsync(); // genera pedido.ID

            // 6) Crear detalles del pedido
            foreach (var i in itemsCarrito)
            {
                _db.DetallePedidos.Add(new DetallePedido
                {
                    PedidoID = pedido.ID,
                    ProductoId = i.Producto.Id, // asegúrate de no tener nulls
                    Cantidad = i.Cantidad,
                    Precio = i.Precio
                });
            }
            await _db.SaveChangesAsync();

            // 7) Limpiar carrito (opcional)
            if (itemsCarrito.Any())
            {
                _db.Carritos.RemoveRange(itemsCarrito);
                await _db.SaveChangesAsync();
            }

            // 8) Redirección final:
            //    - Admin: al detalle del pedido
            //    - Usuario: solo mensaje de éxito
            if (User.IsInRole("Admin"))
                return RedirectToAction("Detalle", "Pedido", new { id = pedido.ID });

            return View("Confirmacion"); // vista simple con “¡Compra exitosa!”
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult CancelarPago() => View("Cancelacion");
    }

    // === modelos mínimos para deserializar la orden de PayPal ===
    public class PayPalOrderResponse
    {
        [JsonPropertyName("id")] public string Id { get; set; }
        [JsonPropertyName("status")] public string Status { get; set; }
        [JsonPropertyName("links")] public List<PayPalLink> Links { get; set; } = new();
    }

    public class PayPalLink
    {
        [JsonPropertyName("href")] public string Href { get; set; }
        [JsonPropertyName("rel")] public string Rel { get; set; }
        [JsonPropertyName("method")] public string Method { get; set; }
    }
}
