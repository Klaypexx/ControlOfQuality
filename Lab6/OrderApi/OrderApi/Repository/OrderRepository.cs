using System.Collections.Concurrent;
using OrderApi.Models;

namespace OrderApi.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ConcurrentDictionary<int, Order> _orders = new();
        private int _nextId = 0;

        public Order AddOrder(Order order)
        {
            order.OrderId = _nextId++;
            _orders[order.OrderId] = order;
            return order;
        }

        public Order GetOrder(int id)
        {
            _orders.TryGetValue(id, out var order);
            return order;
        }

        public IEnumerable<Order> GetAllOrders()
        {
            return _orders.Values;
        }

        public bool RemoveOrder(int id)
        {
            return _orders.TryRemove(id, out _);
        }

        public void UpdateOrder(Order order)
        {
            _orders[order.OrderId] = order;
        }

        public OrderItem GetOrderItem(int orderId, int productId)
        {
            var order = GetOrder(orderId);
            return order?.Items.FirstOrDefault(x => x.Product.Id == productId);
        }
    }
}