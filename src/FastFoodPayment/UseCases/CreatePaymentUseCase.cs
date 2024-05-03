using FastFoodPayment.DTOs.Endpoints;
using FastFoodPayment.Logger;
using FastFoodPayment.Model;
using FastFoodPayment.Repositories;
using System.Text;
using System.Text.Json;

namespace FastFoodPayment.UseCases;

public class CreatePaymentUseCase
{
    private readonly HttpClient _httpClient;

    public CreatePaymentUseCase(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IResult> CreatePayment(CreatePaymentRequest payment, SqsLogger logger, PaymentRepository paymentRepository)
    {
        try
        {
            var _accessToken = Environment.GetEnvironmentVariable("ACCESS_TOKEN_MERCADO_PAGO") ?? throw new ArgumentNullException("Null Access token");
            var _baseUrl = Environment.GetEnvironmentVariable("BASE_URL_MERCADO_PAGO") ?? throw new ArgumentNullException("Null Base Url");
            var _userId = Environment.GetEnvironmentVariable("USER_ID_MERCADO_PAGO") ?? throw new ArgumentNullException("Null User Id");
            var _externalPosId = Environment.GetEnvironmentVariable("EXTERNAL_POS_ID_MERCADO_PAGO") ?? throw new ArgumentNullException("Null External Pos Id");

            CreatePaymentMPResponse mpRepsonse = null;


            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessToken}");

            var content = new StringContent(JsonSerializer.Serialize(payment), Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync($"/instore/orders/qr/seller/collectors/{_userId}/pos/{_externalPosId}/qrs", content);

            if (result.IsSuccessStatusCode)
            {
                var resultString = await result.Content.ReadAsStringAsync();
                mpRepsonse = JsonSerializer.Deserialize<CreatePaymentMPResponse>(resultString);
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
    }
}
