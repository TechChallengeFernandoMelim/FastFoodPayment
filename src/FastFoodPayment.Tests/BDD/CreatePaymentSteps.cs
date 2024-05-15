using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Amazon.SQS;
using FastFoodPayment.DTOs.Endpoints;
using FastFoodPayment.Logger;
using FastFoodPayment.Repositories;
using FastFoodPayment.UseCases;
using Microsoft.AspNetCore.Mvc;
using Moq.Protected;
using Moq;
using System.Net;
using System.Text;
using TechTalk.SpecFlow;
using FastFoodPayment.Model;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Amazon;
using System.Net.Http;

namespace FastFoodPayment.Tests.BDD;

[Binding]
public class CreatePaymentSteps
{
    private readonly Mock<BasicAWSCredentials> credentialsMock = new Mock<BasicAWSCredentials>("accesskey", "secretekeys");
    private readonly Mock<PaymentRepository> repositoryMock = new Mock<PaymentRepository>(new Mock<IAmazonDynamoDB>().Object);
    private readonly Mock<AmazonSQSClient> sqsAwsMock;
    private readonly SqsLogger loggerMock;
    private readonly HttpClient httpClientMock;
    private readonly CreatePaymentUseCase useCase;
    private CreatePaymentRequest paymentRequest;
    private IResult result;
    private CreatePaymentMPResponse mpResponse;

    public CreatePaymentSteps()
    {
        mpResponse = new CreatePaymentMPResponse
        {
            QrData = "qrteste",
            InStoreOrderId = "instoreteste"
        };

        sqsAwsMock = new Mock<AmazonSQSClient>(credentialsMock.Object, RegionEndpoint.USEast1);

        loggerMock = new SqsLogger(sqsAwsMock.Object);

        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        httpResponseMessage.Content = new StringContent(JsonSerializer.Serialize(mpResponse), Encoding.UTF8, "application/json");
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage);

        httpClientMock = new HttpClient(mockHttpMessageHandler.Object);



        repositoryMock.Setup(x => x.CreatePayment(It.IsAny<Payment>())).ReturnsAsync(true);

        useCase = new CreatePaymentUseCase(httpClientMock);
    }

    [Given(@"a valid payment request with description ""(.*)"", external reference ""(.*)"", notification URL ""(.*)"", title ""(.*)"", total amount (\d+), and items with title ""(.*)"", quantity (\d+), total amount (\d+), unit measure ""(.*)"", and unit price (\d+)")]
    public void GivenAValidPaymentRequest(string description, string externalReference, string notificationUrl, string title, int totalAmount, string itemTitle, int quantity, int itemTotalAmount, string unitMeasure, int unitPrice)
    {
        var items = new List<Item>
        {
            new Item
            {
                Title = itemTitle,
                Quantity = quantity,
                TotalAmount = itemTotalAmount,
                UnitMeasure = unitMeasure,
                UnitPrice = unitPrice
            }
        };

        paymentRequest = new CreatePaymentRequest
        {
            Description = description,
            ExternalReference = externalReference,
            NotificationUrl = notificationUrl,
            Title = title,
            TotalAmount = totalAmount,
            Items = items
        };
    }

    [Given(@"the Mercado Pago environment variables are set")]
    public void GivenTheMercadoPagoEnvironmentVariablesAreSet()
    {
        Environment.SetEnvironmentVariable("BASE_URL_MERCADO_PAGO", "http://teste.com");
        Environment.SetEnvironmentVariable("ACCESS_TOKEN_MERCADO_PAGO", "teste");
        Environment.SetEnvironmentVariable("USER_ID_MERCADO_PAGO", "teste");
        Environment.SetEnvironmentVariable("EXTERNAL_POS_ID_MERCADO_PAGO", "teste");
    }

    [When(@"a customer attempts to create a payment")]
    public async Task WhenAnEmployeeAttemptsToCreateAPayment()
    {
        result = await useCase.CreatePayment(paymentRequest, loggerMock, repositoryMock.Object);
    }

    [Then(@"the system should return a QR code data and in-store order ID")]
    public void ThenTheSystemShouldReturnAQRCodeDataAndInStoreOrderID()
    {
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<CreatePaymentMPResponse>>(result);
        var okResult = (Microsoft.AspNetCore.Http.HttpResults.Ok<CreatePaymentMPResponse>)result;
        var createPaymentResponse = okResult.Value;
        Assert.NotNull(createPaymentResponse);
        Assert.Equal(mpResponse.QrData, createPaymentResponse.QrData);
        Assert.Equal(mpResponse.InStoreOrderId, createPaymentResponse.InStoreOrderId);
    }

    [Then(@"the payment request details should match the original request")]
    public void ThenThePaymentRequestDetailsShouldMatchTheOriginalRequest()
    {
        Assert.Equal(paymentRequest.Description, paymentRequest.Description);
        Assert.Equal(paymentRequest.ExternalReference, paymentRequest.ExternalReference);
        Assert.Equal(paymentRequest.NotificationUrl, paymentRequest.NotificationUrl);
        Assert.Equal(paymentRequest.Title, paymentRequest.Title);
        Assert.Equal(paymentRequest.TotalAmount, paymentRequest.TotalAmount);
        Assert.Equal(paymentRequest.Items, paymentRequest.Items);

        var firstItem = paymentRequest.Items[0];

        Assert.Equal(firstItem.Title, firstItem.Title);
        Assert.Equal(firstItem.Quantity, firstItem.Quantity);
        Assert.Equal(firstItem.TotalAmount, firstItem.TotalAmount);
        Assert.Equal(firstItem.UnitPrice, firstItem.UnitPrice);
        Assert.Equal(firstItem.UnitMeasure, firstItem.UnitMeasure);
    }
}
