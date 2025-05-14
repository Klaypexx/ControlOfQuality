using System.Collections.Generic;

namespace OrderApi.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
        public Customer OrderCustomer { get; set; }
        public bool IsSubmitted { get; set; } = false;
    }
}