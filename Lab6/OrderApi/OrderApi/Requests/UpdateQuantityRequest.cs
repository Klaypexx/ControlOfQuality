using OrderApi.Models;

namespace OrderApi.Requests
{
    public class UpdateQuantityRequest
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}