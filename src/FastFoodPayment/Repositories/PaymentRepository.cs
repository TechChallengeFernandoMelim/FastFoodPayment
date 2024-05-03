using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using FastFoodPayment.Model;
using System.Net;
using System.Text.Json;
using System.Threading;

namespace FastFoodPayment.Repositories;

public class PaymentRepository(IAmazonDynamoDB dynamoDb)
{
    private static string tableName = Environment.GetEnvironmentVariable("AWS_TABLE_NAME_DYNAMO");

    public virtual async Task<bool> CreatePayment(Payment payment)
    {
        var paymentAsJson = JsonSerializer.Serialize(payment);
        var itemAsDocument = Document.FromJson(paymentAsJson);
        var itemAsAttribute = itemAsDocument.ToAttributeMap();

        var createItemRequest = new PutItemRequest
        {
            TableName = tableName,
            Item = itemAsAttribute
        };

        var response = await dynamoDb.PutItemAsync(createItemRequest);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public virtual async Task<Payment> GetPaymentByPk(string pk)
    {
        var request = new ScanRequest
        {
            TableName = tableName,
            FilterExpression = "pk = :pk",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":pk", new AttributeValue { S = pk } }
                }
        };

        var response = await dynamoDb.ScanAsync(request);

        if (response.Items.Count == 0)
            return null;

        var itemAsDocument = Document.FromAttributeMap(response.Items.First());
        return JsonSerializer.Deserialize<Payment>(itemAsDocument.ToJson());
    }

    public virtual async Task<bool> UpdatePayment(Payment updatedPayment)
    {
        var updateItemRequest = new UpdateItemRequest
        {
            TableName = tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "pk", new AttributeValue { S = updatedPayment.Pk } },
                { "sk", new AttributeValue { S = updatedPayment.Sk } }
            },
            AttributeUpdates = new Dictionary<string, AttributeValueUpdate>
            {
                {
                    "payment_status", new AttributeValueUpdate
                    {
                        Action = AttributeAction.PUT,
                        Value = new AttributeValue { S = updatedPayment.PaymentStatus }
                    }
                }
            }
        };

        var response = await dynamoDb.UpdateItemAsync(updateItemRequest);

        return response.HttpStatusCode == HttpStatusCode.OK;
    }

}
