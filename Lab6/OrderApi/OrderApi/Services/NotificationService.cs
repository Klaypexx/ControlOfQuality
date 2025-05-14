using OrderApi.Models;

namespace OrderApi.Services
{
    public interface INotificationService
    {
        void NotifyCustomer(Customer customer, string message);
        void SendSmsNotification(Customer customer, string message);
        void LogNotification(Customer customer, string message);
    }

    public class NotificationService : INotificationService
    {
        // Основной метод для Email-уведомлений
        public void NotifyCustomer(Customer customer, string message)
        {
            System.Console.WriteLine($"[Email] Уведомление для {customer.Name}: {message}");
        }

        // Дополнительный метод: отправка SMS уведомления
        public void SendSmsNotification(Customer customer, string message)
        {
            System.Console.WriteLine($"[SMS] Уведомление для {customer.Name}: {message}");
        }

        // Дополнительный метод: логирование уведомления
        public void LogNotification(Customer customer, string message)
        {
            System.Console.WriteLine($"[LOG] Уведомление для {customer.Name}: {message}");
        }
    }
}