using System.Text.Json.Serialization;

namespace FastFoodPayment.Model;

public class Payment
{
    [JsonPropertyName("pk")]
    public string Pk => InStoreOrderId;

    [JsonPropertyName("sk")]
    public string Sk => Pk;

    [JsonPropertyName("qr_data")]
    public string QrData { get; set; }

    [JsonPropertyName("in_store_order_id")]
    public string InStoreOrderId { get; set; }

    [JsonPropertyName("itens_json")]
    public string ItensJson { get; set; }

    [JsonPropertyName("payment_status")]
    public string PaymentStatus { get; set; }

    [JsonPropertyName("creation_date")]
    public DateTime CreationDate { get; set; }
}
