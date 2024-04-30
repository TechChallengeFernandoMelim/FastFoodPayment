using System.Text.Json.Serialization;

namespace FastFoodPayment.DTOs.Endpoints;

public class CreatePaymentMPResponse
{
    [JsonPropertyName("qr_data")]
    public string QrData { get; set; }

    [JsonPropertyName("in_store_order_id")]
    public string InStoreOrderId { get; set; }
}
