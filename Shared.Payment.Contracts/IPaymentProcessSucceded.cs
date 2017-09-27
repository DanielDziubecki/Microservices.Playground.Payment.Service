using System;

namespace Shared.Payment.Contracts
{
    public interface IPaymentProcessSucceded
    {
        Guid? PaymentId { get; set; }
        Guid OrderId { get; set; }
    }
}