using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Proyect_Snake_West.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Proyect_Snake_West.Data;
using Microsoft.EntityFrameworkCore;

namespace Proyect_Snake_West.Controllers
{
    public class PagoController : Controller
    {
        private readonly ILogger<PagoController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public PagoController(ILogger<PagoController> logger,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Create(Decimal monto)
        {

            Pago pago = new Pago();
            pago.UserID = _userManager.GetUserName(User);
            pago.MontoTotal = monto;
            return View(pago);
        }

        [HttpPost]
        public IActionResult Pagar(Pago pago)
        {
            pago.PaymentDate = DateTime.UtcNow;
            _context.Add(pago);

            var itemsCarrito = from o in _context.Carritos select o;
            itemsCarrito = itemsCarrito.
                Include(p => p.Producto).
                Where(s => s.UserID.Equals(pago.UserID) && s.Status.Equals("PENDIENTE"));

            Pedido pedido = new Pedido();
            pedido.UserID = pago.UserID;
            pedido.Total = pago.MontoTotal;
            pedido.pago = pago;
            pedido.Status = "PENDIENTE";
            _context.Add(pedido);

            List<DetallePedido> itemsPedido = new List<DetallePedido>();
            foreach (var item in itemsCarrito.ToList())
            {
                DetallePedido detallePedido = new DetallePedido();
                detallePedido.pedido = pedido;
                detallePedido.Precio = item.Precio;
                detallePedido.Producto = item.Producto;
                detallePedido.Cantidad = item.Cantidad;
                itemsPedido.Add(detallePedido);
            }


            _context.AddRange(itemsPedido);

            foreach (Proforma p in itemsCarrito.ToList())
            {
                p.Status = "PROCESADO";
            }

            _context.UpdateRange(itemsCarrito);

            _context.SaveChanges();

            ViewData["Message"] = "El pago se ha registrado y su pedido nro " + pedido.ID + " esta en camino";
            return View("Create");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
        [HttpPost]
        public async Task<IActionResult> PayWithPaypal(decimal monto)
        {
            // Aquí llamas a tu backend Node o a una clase en C# que use la API de PayPal
            // Suponiendo que llamas al backend en Node que ya funciona:
            var client = new HttpClient();
            var response = await client.GetAsync("http://localhost:3000/create-paypal-order?monto=" + monto); // El backend que ya tienes corriendo con Node.js
            var json = await response.Content.ReadAsStringAsync();

            JObject data = JObject.Parse(json);
            string redirectUrl = data["links"]
            .First(l => l["rel"]!.ToString() == "approve")!["href"]!.ToString();

            return Redirect(redirectUrl);
        }
        public IActionResult ConfirmarPago()
        {
        ViewData["Message"] = "Pago realizado correctamente con PayPal. Gracias por tu compra.";
        return View("Confirmacion");
        }

        public IActionResult CancelarPago()
        {
            ViewData["Message"] = "El pago fue cancelado por el usuario.";
            return View("Cancelacion");
        }

    }
    
}