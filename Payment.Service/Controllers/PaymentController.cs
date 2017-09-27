using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Payment.Model.PayU;
using Payment.Service.Common;
using Payment.Service.Providers.PayU;
using Shared.Payment.Contracts;

namespace Payment.Service.Controllers
{
    [Route("api/payment")]
    public class PaymentController : Controller
    {
        private readonly IPayUClient payUClient;
        private readonly IBusControl busControl;

        public PaymentController(IPayUClient payUClient, IBusControl busControl)
        {
            this.payUClient = payUClient;
            this.busControl = busControl;
        }

        [HttpPost]
        public async Task<IActionResult> PayForOrder([FromBody] PayUOrder payment, [FromHeader(Name = "operationid")]string operationId)
        {
            if (!Guid.TryParse(operationId, out Guid operation))
                return BadRequest("Operation id should be Guid type.");
            
            await busControl.Publish<IPaymentProcessBegin>(new { PaymentId = Guid.NewGuid(), OrderId = payment.ExtOrderId }, context =>
            {
                context.Headers.Set(LogConstansts.Common.OperationId, operation);
                context.Headers.Set(LogConstansts.QueueMessageHeaderNames.Publisher, Request.Path.Value);
            });
            var paymentServiceUrl = await payUClient.GetPayUOrderUrl(payment);
            return Ok(paymentServiceUrl);
        }

        [HttpGet]
        public async Task<IActionResult> PayUPaymentResult()
        {
            return Ok();
        }

        [HttpGet]
        [Route("notify")]
        public async Task<IActionResult> NotifyEndpoint()
        {
            var test = Request;
            return Ok();
        }
    }
}