using FastFoodPayment.DTOs.Endpoints;
using FastFoodPayment.Logger;
using FastFoodPayment.Model;
using FastFoodPayment.Repositories;
using FastFoodPayment.SqsQueues;
using FastFoodPayment.UseCases;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace FastFoodPayment.Endpoints;

public static class PaymentEndpoints
{
    public static void RegisterPaymentsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/CreatePayment", async ([FromBody] CreatePaymentRequest payment, SqsLogger logger, PaymentRepository paymentRepository) =>
        {
            var createPaymentUseCase = new CreatePaymentUseCase(new HttpClient());
            return await createPaymentUseCase.CreatePayment(payment, logger, paymentRepository);
        });

        endpoints.MapPatch("/UpdatePayment/{in_store_order_id}", async (string in_store_order_id, SqsLogger logger, SqsProduction sqsProduction, PaymentRepository paymentRepository) =>
        {
            var updatePaymentUseCase = new UpdatePaymentUseCase();
            return await updatePaymentUseCase.UpdatePayment(in_store_order_id, logger, sqsProduction, paymentRepository);
        });
    }
}
