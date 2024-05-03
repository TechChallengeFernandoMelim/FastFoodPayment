//using System;
//using System.Net;
//using System.Net.Http;
//using System.Text;
//using System.Text.Json;
//using System.Threading.Tasks;
//using FastFoodPayment.DTOs.Endpoints;
//using FastFoodPayment.Endpoints;
//using FastFoodPayment.Logger;
//using FastFoodPayment.Model;
//using FastFoodPayment.Repositories;
//using FastFoodPayment.SqsQueues;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Testing;
//using Microsoft.AspNetCore.Routing;
//using Microsoft.VisualStudio.TestPlatform.TestHost;
//using Moq;
//using Xunit;

//namespace FastFoodPayment.Tests.Endpoints;

//public class PaymentEndpointsTests
//{
//    [Fact]
//    public async Task CreatePayment_ValidInput_ReturnsOkResult()
//    {
//        var endpointRouteBuilder = new Mock<WebApplication>();
//        endpointRouteBuilder.Object.RegisterPaymentsEndpoints();
//        endpointRouteBuilder.


//        // Arrange
//        var payment = new CreatePaymentRequest
//        {
//            // Defina os atributos do pagamento para o teste
//        };

//        var loggerMock = new Mock<SqsLogger>();
//        var paymentRepositoryMock = new Mock<PaymentRepository>();
//        var sqsProductionMock = new Mock<SqsProduction>();

//        // Configurar o mock para simular uma operação bem-sucedida de criação de pagamento
//        paymentRepositoryMock.Setup(repo => repo.CreatePayment(It.IsAny<Payment>())).ReturnsAsync(true);

//        // Act
//        //var result = await PaymentEndpoints.CreatePayment(payment, loggerMock.Object, paymentRepositoryMock.Object, sqsProductionMock.Object);

//        // Assert
//        Assert.IsType<OkObjectResult>(result);
//    }

//    [Fact]
//    public async Task CreatePayment_InvalidInput_ReturnsBadRequestResult()
//    {
//        // Arrange
//        var payment = new CreatePaymentRequest
//        {
//            // Defina um pagamento inválido para o teste
//        };

//        var loggerMock = new Mock<SqsLogger>();
//        var paymentRepositoryMock = new Mock<PaymentRepository>();
//        paymentRepositoryMock.Setup(repo => repo.CreatePayment(It.IsAny<Payment>())).ReturnsAsync(false);

//        var endpoints = new PaymentEndpoints();

//        // Act
//        var result = await endpoints.CreatePayment(payment, loggerMock.Object, paymentRepositoryMock.Object);

//        // Assert
//        Assert.IsType<BadRequestResult>(result);
//    }
//}
