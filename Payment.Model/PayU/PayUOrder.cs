using System;
using System.Collections.Generic;

namespace Payment.Model.PayU
{
    public class PayUOrder
    {
        public string Description { get; set; }
        public Guid ExtOrderId { get; set; }
        public string TotalAmount { get; set; }
        public string CustomerIp { get; set; }
        public string MerchantPosId { get; set; }
        public string ContinueUrl { get; set; }
        public string NotifyUrl { get; set; }
        public string CurrencyCode { get; set; }
        public PayUBuyer Buyer { get; set; }
        public IEnumerable<PayUProduct> Products { get; set; }
    }
}
