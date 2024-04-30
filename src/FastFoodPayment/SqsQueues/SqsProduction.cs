using Amazon.SQS;
using Amazon.SQS.Model;
using FastFoodPayment.Model;

namespace FastFoodPayment.SqsQueues;

public class SqsProduction(AmazonSQSClient sqsClient)
{
    public async Task SendOrderToProduction(Payment payment)
    {
        Dictionary<string, MessageAttributeValue> messageAttributes = new Dictionary<string, MessageAttributeValue>
        {
            { "Service",   new MessageAttributeValue { DataType = "String", StringValue = "FastFoodPayment" } },
            { "InStoreOrderId",   new MessageAttributeValue { DataType = "String", StringValue = payment.InStoreOrderId } },
            { "ItensJson",   new MessageAttributeValue { DataType = "String", StringValue = payment.ItensJson } },
        };

        var sendMessageRequest = new SendMessageRequest
        {
            QueueUrl = Environment.GetEnvironmentVariable("AWS_SQS_PRODUCTION"),
            MessageBody = "Order sent to production",
            MessageGroupId = Environment.GetEnvironmentVariable("AWS_SQS_GROUP_ID_PRODUCTION"),
            MessageAttributes = messageAttributes,
            MessageDeduplicationId = Guid.NewGuid().ToString()
        };

        var sendMessageResponse = await sqsClient.SendMessageAsync(sendMessageRequest);
    }
}
