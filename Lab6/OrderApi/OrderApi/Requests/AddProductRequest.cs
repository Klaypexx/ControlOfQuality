using OrderApi.Models;

namespace OrderApi.Requests
{
    public class AddProductRequest
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}