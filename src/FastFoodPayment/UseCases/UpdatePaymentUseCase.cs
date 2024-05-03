using FastFoodPayment.Logger;
using FastFoodPayment.Repositories;
using FastFoodPayment.SqsQueues;

namespace FastFoodPayment.UseCases;

public class UpdatePaymentUseCase()
{
    public async Task<IResult> UpdatePayment(string in_store_order_id, SqsLogger logger, SqsProduction sqsProduction, PaymentRepository paymentRepository)
    {
        try
        {
            var payment = await paymentRepository.GetPaymentByPk(in_store_order_id);

            if (payment.PaymentStatus == "Paid")
                return Results.BadRequest("Pagamento desse pedido já foi efetuado.");

            if (payment is null)
                throw new Exception("Pagamento não pode ser nulo");

            payment.PaymentStatus = "Paid";
            await paymentRepository.UpdatePayment(payment);

            await sqsProduction.SendOrderToProduction(payment);

            return Results.Ok();
        }
        catch (Exception ex)
        {
            await logger.Log(ex.StackTrace, ex.Message, ex.ToString());
            return Results.BadRequest();
        }
    }
}
