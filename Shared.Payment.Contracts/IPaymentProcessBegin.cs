using System;

namespace Shared.Payment.Contracts
{
    public interface IPaymentProcessBegin
    {
        Guid? PaymentId { get; set; }
        Guid OrderId { get; set; }
    }
}