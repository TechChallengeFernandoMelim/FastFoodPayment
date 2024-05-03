using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2;
using FastFoodPayment.Model;
using FastFoodPayment.Repositories;
using Moq;
using System.Net;

namespace FastFoodPayment.Tests.Repositories;

public class PaymentRepositoryTests
{
    [Fact]
    public async Task CreatePayment_ValidInput_Success()
    {
        // Arrange
        var payment = new Payment
        {
            QrData = "Test"
        };

        var dynamoDbMock = new Mock<IAmazonDynamoDB>();
        dynamoDbMock.Setup(d => d.PutItemAsync(It.IsAny<PutItemRequest>(), default))
                    .ReturnsAsync(new PutItemResponse { HttpStatusCode = HttpStatusCode.OK });

        var repository = new PaymentRepository(dynamoDbMock.Object);

        // Act
        var result = await repository.CreatePayment(payment);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CreatePayment_InvalidInput_Success()
    {
        // Arrange
        var payment = new Payment
        {
            QrData = "Test"
        };

        var dynamoDbMock = new Mock<IAmazonDynamoDB>();
        dynamoDbMock.Setup(d => d.PutItemAsync(It.IsAny<PutItemRequest>(), default))
                    .ReturnsAsync(new PutItemResponse { HttpStatusCode = HttpStatusCode.BadRequest });

        var repository = new PaymentRepository(dynamoDbMock.Object);

        // Act
        var result = await repository.CreatePayment(payment);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetPaymentByPk_ExistingItem_ReturnsPayment()
    {
        // Arrange
        var pk = "example_pk";
        var expectedPayment = new Payment { QrData = "teste" };

        var dynamoDbMock = new Mock<IAmazonDynamoDB>();
        dynamoDbMock.Setup(d => d.ScanAsync(It.IsAny<ScanRequest>(), default))
                    .ReturnsAsync(new ScanResponse
                    {
                        Items = new List<Dictionary<string, AttributeValue>>
                        {
                            new Dictionary<string, AttributeValue>
                            {
                                {"qr_data", new AttributeValue(expectedPayment.QrData) }
                            }
                        }
                    });

        var repository = new PaymentRepository(dynamoDbMock.Object);

        // Act
        var result = await repository.GetPaymentByPk(pk);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedPayment.QrData, result.QrData);
    }

    [Fact]
    public async Task GetPaymentByPk_NonExistingItem_ReturnsNull()
    {
        // Arrange
        var pk = "non_existing_pk";

        var dynamoDbMock = new Mock<IAmazonDynamoDB>();
        dynamoDbMock.Setup(d => d.ScanAsync(It.IsAny<ScanRequest>(), default))
                    .ReturnsAsync(new ScanResponse { Items = new List<Dictionary<string, AttributeValue>>() });

        var repository = new PaymentRepository(dynamoDbMock.Object);

        // Act
        var result = await repository.GetPaymentByPk(pk);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdatePayment_Success_ReturnsTrue()
    {
        // Arrange
        var updatedPayment = new Payment();

        var dynamoDbMock = new Mock<IAmazonDynamoDB>();
        dynamoDbMock.Setup(d => d.UpdateItemAsync(It.IsAny<UpdateItemRequest>(), default))
                    .ReturnsAsync(new UpdateItemResponse { HttpStatusCode = HttpStatusCode.OK });

        var repository = new PaymentRepository(dynamoDbMock.Object);

        // Act
        var result = await repository.UpdatePayment(updatedPayment);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task UpdatePayment_Failure_ReturnsFalse()
    {
        // Arrange
        var updatedPayment = new Payment();

        var dynamoDbMock = new Mock<IAmazonDynamoDB>();
        dynamoDbMock.Setup(d => d.UpdateItemAsync(It.IsAny<UpdateItemRequest>(), default))
                    .ReturnsAsync(new UpdateItemResponse { HttpStatusCode = HttpStatusCode.BadRequest });

        var repository = new PaymentRepository(dynamoDbMock.Object);

        // Act
        var result = await repository.UpdatePayment(updatedPayment);

        // Assert
        Assert.False(result);
    }
}
