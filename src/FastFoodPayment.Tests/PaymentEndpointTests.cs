using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Amazon.SQS;
using FastFoodPayment.DTOs.Endpoints;
using FastFoodPayment.Endpoints;
using FastFoodPayment.Logger;
using FastFoodPayment.Repositories;
using FastFoodPayment.SqsQueues;
using FastFoodPayment.UseCases;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace FastFoodPayment.Tests;

public class PaymentEndpointTests
{
    Mock<BasicAWSCredentials> _credentialsMock;

    public PaymentEndpointTests()
    {
        _credentialsMock = new Mock<BasicAWSCredentials>("accesskey", "secretekeys");
    }
    [Fact]
    public async Task CreatePayment_Should_Return_OkResult()
    {
        // Arrange
        var sqsProductionMock = new Mock<SqsProduction>(new Mock<AmazonSQSClient>(_credentialsMock.Object, RegionEndpoint.USEast1).Object);
        var sqsLoggerMock = new Mock<SqsLogger>(new Mock<AmazonSQSClient>(_credentialsMock.Object, RegionEndpoint.USEast1).Object);
        var paymentRepositoryMock = new Mock<PaymentRepository>(new Mock<IAmazonDynamoDB>().Object);

        var builder = WebApplication.CreateBuilder();
        builder.Services.AddSingleton<SqsLogger>(sqsLoggerMock.Object);
        builder.Services.AddSingleton<SqsProduction>(sqsProductionMock.Object);
        builder.Services.AddSingleton<PaymentRepository>(paymentRepositoryMock.Object);

        var endpoints = builder.Build();

        // Act
        endpoints.RegisterPaymentsEndpoints();
    }
}
