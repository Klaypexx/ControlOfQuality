using OrderApi.Models;
using OrderApi.Repository;
using System.Collections.Generic;
using System.Linq;

namespace OrderApi.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentService _paymentService;
        private readonly INotificationService _notificationService;

        public OrderService(IOrderRepository orderRepository,
                            IPaymentService paymentService,
                            INotificationService notificationService)
        {
            _orderRepository = orderRepository;
            _paymentService = paymentService;
            _notificationService = notificationService;
        }

        public Order CreateOrder(Customer customer)
        {
            var order = new Order { OrderCustomer = customer };
            _orderRepository.AddOrder(order);
            return order;
        }

        public Order GetOrder(int orderId)
        {
            return _orderRepository.GetOrder(orderId);
        }

        public IEnumerable<Order> GetAllOrders()
        {
            return _orderRepository.GetAllOrders();
        }

        public Order AddProduct(int orderId, Product product, int quantity)
        {
            var order = _orderRepository.GetOrder(orderId);
            if (order == null)
                return null;
            var existingItem = _orderRepository.GetOrderItem(orderId, product.Id);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var newItem = new OrderItem { Product = product, Quantity = quantity };
                order.Items.Add(newItem);
            }
            _orderRepository.UpdateOrder(order);
            return order;
        }

        public bool RemoveProduct(int orderId, int productId)
        {
            var order = _orderRepository.GetOrder(orderId);
            if (order == null)
                return false;
            var existingItem = _orderRepository.GetOrderItem(orderId, productId);
            if (existingItem != null)
            {
                order.Items.Remove(existingItem);
                _orderRepository.UpdateOrder(order);
                return true;
            }
            return false;
        }

        public Order UpdateQuantity(int orderId, Product product, int newQuantity)
        {
            var order = _orderRepository.GetOrder(orderId);
            if (order == null)
                return null;
            var existingItem = _orderRepository.GetOrderItem(orderId, product.Id);
            if (existingItem != null)
            {
                if (newQuantity <= 0)
                {
                    order.Items.Remove(existingItem);
                }
                else
                {
                    existingItem.Quantity = newQuantity;
                }
                _orderRepository.UpdateOrder(order);
            }
            return order;
        }

        public decimal CalculateTotal(int orderId)
        {
            var order = _orderRepository.GetOrder(orderId);
            if (order == null)
                return 0;
            return order.Items.Sum(x => x.Product.Price * x.Quantity);
        }

        public bool SubmitOrder(int orderId)
        {
            var order = _orderRepository.GetOrder(orderId);
            if (order == null)
                return false;
            if (order.IsSubmitted || order.Items.Count == 0)
                return false;

            bool paymentResult = _paymentService.ProcessPayment(order);
            if (!paymentResult)
                return false;

            _notificationService.NotifyCustomer(order.OrderCustomer, "Ваш заказ успешно оформлен.");
            _notificationService.LogNotification(order.OrderCustomer, "Заказ подтверждён и оплачен.");
            _notificationService.SendSmsNotification(order.OrderCustomer, "Ваш заказ принят в обработку.");

            order.IsSubmitted = true;
            _orderRepository.UpdateOrder(order);
            return true;
        }
    }
}