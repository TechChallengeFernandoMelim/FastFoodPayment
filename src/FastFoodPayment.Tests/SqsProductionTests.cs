using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using FastFoodPayment.Model;
using FastFoodPayment.SqsQueues;
using Moq;

namespace FastFoodPayment.Tests;

public class SqsProductionTests
{
    Mock<BasicAWSCredentials> _credentialsMock;

    public SqsProductionTests()
    {
        _credentialsMock = new Mock<BasicAWSCredentials>("accesskey", "secretekeys");
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

        var sqsClientMock = new Mock<AmazonSQSClient>(_credentialsMock.Object, RegionEndpoint.USEast1);
        sqsClientMock.Setup(s => s.SendMessageAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new SendMessageResponse());

        var sqsService = new SqsProduction(sqsClientMock.Object);

        // Act
        await sqsService.SendOrderToProduction(payment);

        // Assert
        sqsClientMock.Verify(s => s.SendMessageAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
