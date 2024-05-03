using Amazon.DynamoDBv2;
using Amazon.SQS;
using FastFoodPayment.DTOs.Endpoints;
using FastFoodPayment.Logger;
using FastFoodPayment.Model;
using FastFoodPayment.Repositories;
using FastFoodPayment.UseCases;
using Microsoft.AspNetCore.Routing;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;
using System.Text.Json;

namespace FastFoodPayment.Tests;

public class CreatePaymentTests()
{
    [Fact]
    public async Task CreatePayment_Error_ReturnsBadRequest()
    {
        // Arrange
        var mockSqsAws = new Mock<AmazonSQSClient>();
        var mockLogger = new SqsLogger(mockSqsAws.Object);
        var mockRepository = new PaymentRepository(new Mock<IAmazonDynamoDB>().Object);

        var paymentRequest = new CreatePaymentRequest { };

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        var mpResponse = new CreatePaymentMPResponse { };

        httpResponseMessage.Content = new StringContent(JsonSerializer.Serialize(mpResponse), Encoding.UTF8, "application/json");



        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK });

        var client = new HttpClient(mockHttpMessageHandler.Object);

        var createPaymentUseCase = new CreatePaymentUseCase(client);

        // Act
        var result = await createPaymentUseCase.CreatePayment(paymentRequest, mockLogger, mockRepository);

        // Assert
        Assert.True(((Microsoft.AspNetCore.Http.HttpResults.BadRequest)result).StatusCode == 400);
    }

    [Fact]
    public async Task CreatePayment_Ok_ReturnsOk()
    {
        // Arrange
        var mockSqsAws = new Mock<AmazonSQSClient>();
        var mockLogger = new SqsLogger(mockSqsAws.Object);
        var mockRepository = new Mock<PaymentRepository>(new Mock<IAmazonDynamoDB>().Object);

        var paymentRequest = new CreatePaymentRequest { };

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        var mpResponse = new CreatePaymentMPResponse
        {
            QrData = "qrteste",
            InStoreOrderId = "instoreteste"
        };

        httpResponseMessage.Content = new StringContent(JsonSerializer.Serialize(mpResponse), Encoding.UTF8, "application/json");

        mockRepository.Setup(x => x.CreatePayment(It.IsAny<Payment>())).ReturnsAsync(true);

        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage);

        var client = new HttpClient(mockHttpMessageHandler.Object);

        var createPaymentUseCase = new CreatePaymentUseCase(client);

        Environment.SetEnvironmentVariable("BASE_URL_MERCADO_PAGO", "http://teste.com");
        Environment.SetEnvironmentVariable("ACCESS_TOKEN_MERCADO_PAGO", "teste");
        Environment.SetEnvironmentVariable("USER_ID_MERCADO_PAGO", "teste");
        Environment.SetEnvironmentVariable("EXTERNAL_POS_ID_MERCADO_PAGO", "teste");
        // Act
        var result = await createPaymentUseCase.CreatePayment(paymentRequest, mockLogger, mockRepository.Object);

        // Assert
        Assert.Equal(((Microsoft.AspNetCore.Http.HttpResults.Ok<CreatePaymentMPResponse>)result).StatusCode, 200);
        Assert.Equal(mpResponse.QrData, ((Microsoft.AspNetCore.Http.HttpResults.Ok<CreatePaymentMPResponse>)result).Value.QrData);
        Assert.Equal(mpResponse.InStoreOrderId, ((Microsoft.AspNetCore.Http.HttpResults.Ok<CreatePaymentMPResponse>)result).Value.InStoreOrderId);
    }
}
