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

        // Use an HTTP context to simulate a request to your endpoint
        //var httpContext = new DefaultHttpContext();
        //httpContext.Request.Method = "POST";
        //httpContext.Request.Path = "/CreatePayment";

        //var endpointDataSource = endpoints.Services.GetRequiredService<EndpointDataSource>();

        //var matchingEndpoints = endpointDataSource.Endpoints.Where(e =>
        //{
        //    var endpoint = e as RouteEndpoint;
        //    var httpMethodMetadata = e.Metadata.GetMetadata<HttpMethodMetadata>();
        //    return endpoint != null &&
        //           httpMethodMetadata != null &&
        //           httpMethodMetadata.HttpMethods.Contains(httpContext.Request.Method) &&
        //           endpoint.RoutePattern.RawText == httpContext.Request.Path;
        //});

        //var endpoint = matchingEndpoints.FirstOrDefault();

        //Assert.NotNull(endpoint);

        // Execute the endpoint
        //var result = await endpoint.RequestDelegate.Invoke(httpContext) as IActionResult;

        //// Assert
        //Assert.IsType<OkResult>(result);
    }
}
