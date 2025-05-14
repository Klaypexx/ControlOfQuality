using OrderApi.Models;

namespace OrderApi.Services
{
    public interface IOrderService
    {
        Order CreateOrder(Customer customer);
        Order AddProduct(int orderId, Product product, int quantity);
        bool RemoveProduct(int orderId, int productId);
        Order UpdateQuantity(int orderId, Product product, int newQuantity);
        decimal CalculateTotal(int orderId);
        bool SubmitOrder(int orderId);
        Order GetOrder(int orderId);
        IEnumerable<Order> GetAllOrders();
    }
}