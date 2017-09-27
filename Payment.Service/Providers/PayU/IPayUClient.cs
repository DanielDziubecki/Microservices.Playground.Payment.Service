using System.Threading.Tasks;
using Payment.Model.PayU;

namespace Payment.Service.Providers.PayU
{
    public interface IPayUClient
    {
        Task<string> GetPayUOrderUrl(PayUOrder order);
    }
}