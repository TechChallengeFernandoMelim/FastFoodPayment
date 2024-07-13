using FastFoodPayment.Logger;
using FastFoodPayment.Model;
using FastFoodPayment.Repositories;
using FastFoodPayment.SqsQueues;

namespace FastFoodPayment.UseCases;

public class UpdatePaymentUseCase()
{
    public async Task<IResult> UpdatePayment(string in_store_order_id, SqsLogger logger, SqsProduction sqsProduction, PaymentRepository paymentRepository)
    {
        var payment = await paymentRepository.GetPaymentByPk(in_store_order_id);

        if (payment is null)
            return Results.BadRequest("Pagamento não pode ser nulo");

        if (payment.PaymentStatus == "Paid")
            return Results.BadRequest("Pagamento desse pedido já foi efetuado.");

        payment.PaymentStatus = "Paid";

        try
        {
            await paymentRepository.UpdatePayment(payment);

            await sqsProduction.SendOrderToProduction(payment);

            return Results.Ok();

        }
        catch (Exception ex)
        {
            await logger.Log(ex.StackTrace, ex.Message, ex.ToString());
            await CancelOrder(payment, paymentRepository, sqsProduction);
            return Results.BadRequest();
        }
    }

    public async Task CancelOrder(Payment payment, PaymentRepository paymentRepository, SqsProduction sqsProduction)
    {
        payment.PaymentStatus = "Canceled";
        await paymentRepository.UpdatePayment(payment);
        await sqsProduction.SendOrderToProduction(payment);
    }
}
