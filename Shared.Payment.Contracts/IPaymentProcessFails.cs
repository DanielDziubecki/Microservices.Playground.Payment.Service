using System;

namespace Shared.Payment.Contracts
{
    public interface IPaymentProcessFails
    {
        Guid? PaymentId { get; set; }
        Guid OrderId { get; set; }
    }
}