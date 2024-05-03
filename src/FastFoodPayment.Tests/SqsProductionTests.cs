using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using FastFoodPayment.Model;
using FastFoodPayment.SqsQueues;
using Moq;

namespace FastFoodPayment.Tests;

public class SqsProductionTests
{
    Mock<AWSCredentials> _credentialsMock;

    public SqsProductionTests()
    {
        string accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_DYNAMO");
        string secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_KEY_DYNAMO");

        _credentialsMock = new Mock<AWSCredentials>(new BasicAWSCredentials(accessKey, secretKey));
    }

    [Fact]
    public async Task SendOrderToProduction_ValidInput_Success()
    {
        // Arrange
        var payment = new Payment
        {
            InStoreOrderId = "12345",
            ItensJson = "{\"item\":\"description\"}"
        };

        var sqsClientMock = new Mock<AmazonSQSClient>(_credentialsMock.Object);
        sqsClientMock.Setup(s => s.SendMessageAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new SendMessageResponse());

        var sqsService = new SqsProduction(sqsClientMock.Object);

        // Act
        await sqsService.SendOrderToProduction(payment);

        // Assert
        sqsClientMock.Verify(s => s.SendMessageAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
