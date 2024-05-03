using Amazon.DynamoDBv2;
using Amazon.SQS;
using FastFoodPayment.Logger;
using FastFoodPayment.Model;
using FastFoodPayment.Repositories;
using FastFoodPayment.SqsQueues;
using FastFoodPayment.UseCases;
using Microsoft.AspNetCore.Http;
using Moq;

namespace FastFoodPayment.Tests;

public class UpdatePaymentTests
{
    [Fact]
    public async Task UpdatePayment_PaidStatus_ReturnsBadRequest()
    {
        // Arrange
        var inStoreOrderId = "test_order_id";
        var loggerMock = new Mock<SqsLogger>(new Mock<AmazonSQSClient>().Object);
        var sqsProductionMock = new Mock<SqsProduction>(new Mock<AmazonSQSClient>().Object);
        var paymentRepositoryMock = new Mock<PaymentRepository>(new Mock<IAmazonDynamoDB>().Object);

        paymentRepositoryMock.Setup(x => x.UpdatePayment(It.IsAny<Payment>())).ReturnsAsync(true);

        var payment = new Payment { PaymentStatus = "Paid" };
        paymentRepositoryMock.Setup(repo => repo.GetPaymentByPk(It.IsAny<string>()))
                             .ReturnsAsync(payment);

        var useCase = new UpdatePaymentUseCase();

        // Act
        var result = await useCase.UpdatePayment(inStoreOrderId, loggerMock.Object, sqsProductionMock.Object, paymentRepositoryMock.Object);

        // Assert
        Assert.Equal(((Microsoft.AspNetCore.Http.HttpResults.BadRequest<string>)result).StatusCode, 400);
        Assert.Equal(((Microsoft.AspNetCore.Http.HttpResults.BadRequest<string>)result).Value, "Pagamento desse pedido já foi efetuado.");
    }

    [Fact]
    public async Task UpdatePayment_NullPayment_ReturnsBadRequest()
    {
        // Arrange
        var inStoreOrderId = "test_order_id";
        var loggerMock = new Mock<SqsLogger>(new Mock<AmazonSQSClient>().Object);
        var sqsProductionMock = new Mock<SqsProduction>(new Mock<AmazonSQSClient>().Object);
        var paymentRepositoryMock = new Mock<PaymentRepository>(new Mock<IAmazonDynamoDB>().Object);

        paymentRepositoryMock.Setup(repo => repo.GetPaymentByPk(inStoreOrderId))
                             .ReturnsAsync((Payment)null);

        var useCase = new UpdatePaymentUseCase();

        // Act
        var result = await useCase.UpdatePayment(inStoreOrderId, loggerMock.Object, sqsProductionMock.Object, paymentRepositoryMock.Object);

        // Assert
        Assert.Equal(((Microsoft.AspNetCore.Http.HttpResults.BadRequest)result).StatusCode, 400);
    }

    [Fact]
    public async Task UpdatePayment_Success_ReturnsOk()
    {
        // Arrange
        var inStoreOrderId = "test_order_id";
        var loggerMock = new Mock<SqsLogger>(new Mock<AmazonSQSClient>().Object);
        var sqsProductionMock = new Mock<SqsProduction>(new Mock<AmazonSQSClient>().Object);
        var paymentRepositoryMock = new Mock<PaymentRepository>(new Mock<IAmazonDynamoDB>().Object);

        var payment = new Payment { PaymentStatus = "Pending" };
        paymentRepositoryMock.Setup(repo => repo.GetPaymentByPk(inStoreOrderId))
                             .ReturnsAsync(payment);

        var useCase = new UpdatePaymentUseCase();

        // Act
        var result = await useCase.UpdatePayment(inStoreOrderId, loggerMock.Object, sqsProductionMock.Object, paymentRepositoryMock.Object);

        // Assert
        Assert.Equal(((Microsoft.AspNetCore.Http.HttpResults.Ok)result).StatusCode, 200);
    }

    [Fact]
    public async Task UpdatePayment_Exception_ReturnsBadRequest()
    {
        // Arrange
        var inStoreOrderId = "test_order_id";
        var loggerMock = new Mock<SqsLogger>(new Mock<AmazonSQSClient>().Object);
        var sqsProductionMock = new Mock<SqsProduction>(new Mock<AmazonSQSClient>().Object);
        var paymentRepositoryMock = new Mock<PaymentRepository>(new Mock<IAmazonDynamoDB>().Object);

        paymentRepositoryMock.Setup(repo => repo.GetPaymentByPk(inStoreOrderId))
                             .Throws(new Exception("Test exception"));

        var useCase = new UpdatePaymentUseCase();

        // Act
        var result = await useCase.UpdatePayment(inStoreOrderId, loggerMock.Object, sqsProductionMock.Object, paymentRepositoryMock.Object);

        // Assert
        Assert.Equal(((Microsoft.AspNetCore.Http.HttpResults.BadRequest)result).StatusCode, 400);
    }
}
