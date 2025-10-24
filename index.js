require("dotenv").config({ path: __dirname + "/.env" }); // <-- muy importante

const express = require("express");
const paypal = require("./services/paypal");
const app = express();

app.use(express.urlencoded({ extended: false }));
app.use(express.json());

app.get("/create-paypal-order", async (req, res) => {
  try {
    const monto = req.query.monto;
    if (!monto) return res.status(400).json({ error: "monto requerido" });

    const order = await paypal.createOrderWithAmount(monto);
    return res.json(order);
  } catch (error) {
    const details = error?.response?.data || error?.message || String(error);
    console.error("Error en /create-paypal-order:", details);
    return res.status(500).json({ error: "Error al crear la orden", details });
  }
});

app.get("/complete-order", async (req, res) => {
  try {
    const token = req.query.token;
    const data = await paypal.capturePayment(token);
    res.json(data);
  } catch (error) {
    const details = error?.response?.data || error?.message || String(error);
    console.error("Error en /complete-order:", details);
    res.status(500).json({ error: "Error al capturar el pago", details });
  }
});

app.listen(3000, () => console.log("Server is running on port 3000"));
