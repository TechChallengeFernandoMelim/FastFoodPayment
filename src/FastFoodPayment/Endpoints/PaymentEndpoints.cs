using FastFoodPayment.DTOs.Endpoints;
using FastFoodPayment.Logger;
using FastFoodPayment.Model;
using FastFoodPayment.Repositories;
using FastFoodPayment.SqsQueues;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace FastFoodPayment.Endpoints;

public static class PaymentEndpoints
{

    private static string _accessToken = Environment.GetEnvironmentVariable("ACCESS_TOKEN_MERCADO_PAGO") ?? throw new ArgumentNullException("Null Access token");
    private static string _baseUrl = Environment.GetEnvironmentVariable("BASE_URL_MERCADO_PAGO") ?? throw new ArgumentNullException("Null Base Url");
    private static string _userId = Environment.GetEnvironmentVariable("USER_ID_MERCADO_PAGO") ?? throw new ArgumentNullException("Null User Id");
    private static string _externalPosId = Environment.GetEnvironmentVariable("EXTERNAL_POS_ID_MERCADO_PAGO") ?? throw new ArgumentNullException("Null External Pos Id");

    public static void RegisterPaymentsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/CreatePayment", async ([FromBody] CreatePaymentRequest payment, SqsLogger logger, PaymentRepository paymentRepository) =>
        {
            try
            {
                CreatePaymentMPResponse mpRepsonse = null;

                using (var httpRequest = new HttpClient())
                {
                    httpRequest.BaseAddress = new Uri(_baseUrl);
                    httpRequest.DefaultRequestHeaders.Clear();
                    httpRequest.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessToken}");

                    var content = new StringContent(JsonSerializer.Serialize(payment), Encoding.UTF8, "application/json");
                    var result = await httpRequest.PostAsync($"/instore/orders/qr/seller/collectors/{_userId}/pos/{_externalPosId}/qrs", content);

                    if (result.IsSuccessStatusCode)
                    {
                        var resultString = await result.Content.ReadAsStringAsync();
                        mpRepsonse = JsonSerializer.Deserialize<CreatePaymentMPResponse>(resultString);
                    }
                }

                if (mpRepsonse != null)
                {
                    var newPayment = new Payment()
                    {
                        QrData = mpRepsonse.QrData,
                        InStoreOrderId = mpRepsonse.InStoreOrderId,
                        ItensJson = JsonSerializer.Serialize(payment.Items),
                        PaymentStatus = "AwaitingPayment",
                        CreationDate = DateTime.Now
                    };

                    await paymentRepository.CreatePayment(newPayment);
                    return Results.Ok(mpRepsonse);
                }

                throw new Exception("Ocorreu algum erro ao criar pagamento. Resposta do Mercado Pago é nula. Rever configurações de conexão com MP.");
            }
            catch (Exception ex)
            {
                await logger.Log(ex.StackTrace, ex.Message, ex.ToString());
                return Results.BadRequest();
            }
        });

        endpoints.MapPatch("/UpdatePayment/{in_store_order_id}", async (string in_store_order_id, SqsLogger logger, SqsProduction sqsProduction, PaymentRepository paymentRepository) =>
        {
            try
            {
                var payment = await paymentRepository.GetPaymentByPk(in_store_order_id);

                if (payment is null)
                    throw new Exception("Pagamento não pode ser nulo");

                payment.PaymentStatus = "Paid";
                await paymentRepository.UpdatePayment(payment);

                await sqsProduction.SendOrderToProduction(payment);

                return Results.Ok();
            }
            catch (Exception ex)
            {
                await logger.Log(ex.StackTrace, ex.Message, ex.ToString());
                return Results.BadRequest();
            }
        });
    }
}
