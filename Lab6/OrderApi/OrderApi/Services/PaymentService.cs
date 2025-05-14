using OrderApi.Models;
using System;
using System.Linq;

namespace OrderApi.Services
{
    public class PaymentService : IPaymentService
    {
        public bool ProcessPayment(Order order)
        {
            Console.WriteLine($"Платеж обработан для клиента {order.OrderCustomer.Name}. Сумма: {order.Items.Sum(x => x.Product.Price * x.Quantity)}");
            return true;
        }

        public bool RefundPayment(Order order)
        {
            Console.WriteLine($"Платеж возвращён для заказа клиента {order.OrderCustomer.Name}");
            return true;
        }

        public string GetPaymentStatus(Order order)
        {
            return "Paid";
        }
    }
}