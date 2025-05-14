using OrderApi.Models;
using System.Collections.Generic;

namespace OrderApi.Repository
{
    public interface IOrderRepository
    {
        Order AddOrder(Order order);
        Order GetOrder(int id);
        IEnumerable<Order> GetAllOrders();
        bool RemoveOrder(int id);
        void UpdateOrder(Order order);

        // Метод поиска OrderItem по productId в заказе
        OrderItem GetOrderItem(int orderId, int productId);
    }
}