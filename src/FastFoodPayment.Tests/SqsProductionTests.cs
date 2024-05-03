using Amazon.SQS;
using Amazon.SQS.Model;
using FastFoodPayment.Model;
using FastFoodPayment.SqsQueues;
using Moq;

namespace FastFoodPayment.Tests;

public class SqsProductionTests
{
    [Fact]
    public async Task SendOrderToProduction_ValidInput_Success()
    {
        // Arrange
        var payment = new Payment
        {
            InStoreOrderId = "12345",
            ItensJson = "{\"item\":\"description\"}"
        };

        var sqsClientMock = new Mock<AmazonSQSClient>();
        sqsClientMock.Setup(s => s.SendMessageAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new SendMessageResponse());

        var sqsService = new SqsProduction(sqsClientMock.Object);

        // Act
        await sqsService.SendOrderToProduction(payment);

        // Assert
        sqsClientMock.Verify(s => s.SendMessageAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
