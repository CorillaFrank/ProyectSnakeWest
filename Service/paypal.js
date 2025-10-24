const axios = require("axios");

async function generateAccessToken() {
  const url = `${process.env.PAYPAL_BASE_URL}/v1/oauth2/token`;
  try {
    const response = await axios({
      method: "post",
      url,
      headers: { "Content-Type": "application/x-www-form-urlencoded" },
      data: "grant_type=client_credentials",
      auth: {
        username: process.env.PAYPAL_CLIENT_ID,
        password: process.env.PAYPAL_SECRET,
      },
    });
    return response.data.access_token;
  } catch (error) {
    console.error(
      "Error al generar access token:",
      error.response?.data || error.message
    );
    throw error;
  }
}

exports.createOrderWithAmount = async (monto) => {
  try {
    const accessToken = await generateAccessToken();
    const response = await axios({
      url: `${process.env.PAYPAL_BASE_URL}/v2/checkout/orders`,
      method: "post",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${accessToken}`,
      },
      data: {
        intent: "CAPTURE",
        purchase_units: [
          {
            amount: {
              currency_code: "USD",
              value: String(monto), // "10.00"
            },
          },
        ],
        application_context: {
          return_url: `${process.env.BASE_URL}/Pago/ConfirmarPago`,
          cancel_url: `${process.env.BASE_URL}/Pago/CancelarPago`,
          shipping_preference: "NO_SHIPPING",
          user_action: "PAY_NOW",
          brand_name: "SneakerWest",
        },
      },
    });
    return response.data;
  } catch (error) {
    console.error(
      "Error al crear la orden:",
      error.response?.data || error.message
    );
    throw error;
  }
};

exports.capturePayment = async (orderId) => {
  const accessToken = await generateAccessToken();
  const response = await axios({
    url: `${process.env.PAYPAL_BASE_URL}/v2/checkout/orders/${orderId}/capture`,
    method: "post",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${accessToken}`,
    },
  });
  return response.data;
};
