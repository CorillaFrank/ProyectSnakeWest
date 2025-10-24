using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Proyect_Snake_West.Models;
using System.Globalization;
using System.Text.Json;

namespace Proyect_Snake_West.Controllers
{
    using System.Globalization;
// ...

public class PagoController : Controller
{
    private readonly IHttpClientFactory _http;
    private readonly IConfiguration _config;

    public PagoController(IHttpClientFactory http, IConfiguration config)
    {
        _http = http;
        _config = config;
    }

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
        // Muestra EXACTAMENTE lo que responde el Node
        return Content($"Node devolvió {resp.StatusCode}:\n{body}", "text/plain");
    }

    var order = JsonSerializer.Deserialize<PayPalOrderResponse>(
        body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

    var approve = order?.Links?.FirstOrDefault(l => l.Rel == "approve")?.Href;
    if (string.IsNullOrEmpty(approve))
        return Content($"Orden sin link de aprobación:\n{body}", "text/plain");

    return Redirect(approve);
}


    [HttpGet]
    public async Task<IActionResult> ConfirmarPago(string token)
    {
        if (string.IsNullOrEmpty(token))
            return BadRequest("Token inválido.");

        var baseUrl = _config["Node:BaseUrl"];
        var cli = _http.CreateClient();
        var resp = await cli.GetAsync($"{baseUrl}/complete-order?token={token}");

        if (!resp.IsSuccessStatusCode)
            return View("Cancelacion");

        return View("Confirmacion");
    }

    [HttpGet]
    public IActionResult CancelarPago() => View("Cancelacion");
}


    // === modelos mínimos para deserializar la orden de PayPal ===
    public class PayPalOrderResponse
    {
        [JsonPropertyName("id")] public string Id { get; set; }
        [JsonPropertyName("status")] public string Status { get; set; }
        [JsonPropertyName("links")] public List<PayPalLink> Links { get; set; }
    }
    public class PayPalLink
    {
        [JsonPropertyName("href")] public string Href { get; set; }
        [JsonPropertyName("rel")] public string Rel { get; set; }
        [JsonPropertyName("method")] public string Method { get; set; }
    }
}
