using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using FastFoodPayment.Logger;
using Moq;

namespace FastFoodPayment.Tests;

public class SqsLoggerTests
{
    Mock<AWSCredentials> _credentialsMock;

    public SqsLoggerTests()
    {
        string accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_DYNAMO");
        string secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_KEY_DYNAMO");

        _credentialsMock = new Mock<AWSCredentials>(new BasicAWSCredentials(accessKey, secretKey));
    }

    [Fact]
    public async Task Log_ValidInput_Success()
    {
        // Arrange
        var stackTrace = "Test stack trace";
        var message = "Test message";
        var exception = "Test exception";

        var sqsClientMock = new Mock<AmazonSQSClient>(_credentialsMock.Object);
        sqsClientMock.Setup(s => s.SendMessageAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new SendMessageResponse());

        var sqsService = new SqsLogger(sqsClientMock.Object);

        // Act
        await sqsService.Log(stackTrace, message, exception);

        // Assert
        sqsClientMock.Verify(s => s.SendMessageAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
