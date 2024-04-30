using System.Text.Json.Serialization;
using ThirdParty.Json.LitJson;

namespace FastFoodPayment.DTOs.Endpoints;

public record CreatePaymentRequest
{
    [JsonPropertyName("total_amount")]
    public decimal TotalAmount { get; set; }

    [JsonPropertyName("external_reference")]
    public string ExternalReference { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("items")]
    public List<Item> Items { get; set; }

    [JsonPropertyName("notification_url")]
    public string NotificationUrl { get; set; }
}

public record Item
{
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("unit_measure")]
    public string UnitMeasure { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("unit_price")]
    public decimal UnitPrice { get; set; }

    [JsonPropertyName("total_amount")]
    public decimal TotalAmount { get; set; }
}
