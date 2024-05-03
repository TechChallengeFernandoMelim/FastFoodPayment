using Amazon.SQS.Model;
using Amazon.SQS;
using FastFoodPayment.SqsQueues;
using Moq;
using FastFoodPayment.Logger;

namespace FastFoodPayment.Tests;

public class SqsLoggerTests
{
    [Fact]
    public async Task Log_ValidInput_Success()
    {
        // Arrange
        var stackTrace = "Test stack trace";
        var message = "Test message";
        var exception = "Test exception";

        var sqsClientMock = new Mock<AmazonSQSClient>();
        sqsClientMock.Setup(s => s.SendMessageAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new SendMessageResponse());

        var sqsService = new SqsLogger(sqsClientMock.Object);

        // Act
        await sqsService.Log(stackTrace, message, exception);

        // Assert
        sqsClientMock.Verify(s => s.SendMessageAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
