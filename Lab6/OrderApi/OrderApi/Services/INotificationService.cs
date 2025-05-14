using OrderApi.Models;

namespace OrderApi.Services
{
    public interface INotificationService
    {
        void NotifyCustomer(Customer customer, string message);
        void SendSmsNotification(Customer customer, string message);
        void LogNotification(Customer customer, string message);
    }
}