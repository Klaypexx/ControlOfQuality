using Microsoft.AspNetCore.Mvc;
using OrderApi.Models;
using OrderApi.Requests;
using OrderApi.Services;
using System.Linq;

namespace OrderApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("create")]
        public IActionResult CreateOrder([FromBody] Customer customer)
        {
            var order = _orderService.CreateOrder(customer);
            return Ok(new { orderId = order.OrderId, order });
        }

        [HttpPost("{orderId}/add")]
        public IActionResult AddProduct(int orderId, [FromBody] AddProductRequest request)
        {
            var order = _orderService.AddProduct(orderId, request.Product, request.Quantity);
            if (order == null)
                return NotFound("Order not found");
            return Ok(order);
        }

        [HttpPost("{orderId}/remove")]
        public IActionResult RemoveProduct(int orderId, [FromQuery] int productId)
        {
            bool result = _orderService.RemoveProduct(orderId, productId);
            return Ok(result);
        }

        [HttpPost("{orderId}/update")]
        public IActionResult UpdateQuantity(int orderId, [FromBody] UpdateQuantityRequest request)
        {
            var order = _orderService.UpdateQuantity(orderId, request.Product, request.Quantity);
            if (order == null)
                return NotFound("Order not found");
            return Ok(order);
        }

        [HttpGet("{orderId}/total")]
        public IActionResult GetTotal(int orderId)
        {
            decimal total = _orderService.CalculateTotal(orderId);
            return Ok(total);
        }

        [HttpPost("{orderId}/submit")]
        public IActionResult SubmitOrder(int orderId)
        {
            bool result = _orderService.SubmitOrder(orderId);
            if (result)
                return Ok("Order submitted successfully");
            else
                return BadRequest("Failed to submit order");
        }

        [HttpGet]
        public IActionResult GetAllOrders()
        {
            var orders = _orderService.GetAllOrders();
            return Ok(orders);
        }
    }
}