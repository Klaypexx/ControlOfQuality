using OrderApi.Models;

namespace OrderApi.Services
{
    public interface IPaymentService
    {
        bool ProcessPayment(Order order);
        bool RefundPayment(Order order);
        string GetPaymentStatus(Order order);
    }
}