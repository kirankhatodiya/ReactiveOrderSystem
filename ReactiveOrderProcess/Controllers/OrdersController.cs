using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReactiveOrderProcess.Core.Interfaces;
using ReactiveOrderProcess.Core.Models.Requests;

namespace ReactiveOrderProcess.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrdersController(IOrderService orderService) 
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetOrdersAsync();
            return Ok(orders);
        }

        [HttpPost]  
        public async Task<IActionResult> AddTask([FromBody]OrderRequest orderRequest)
        {
            if (orderRequest == null || orderRequest.ProductItems.Count() == 0)
            {
                return BadRequest("At least one order item is required.");
            }
            var result = await _orderService.AddOrderAsync(orderRequest);
            if (result.StatusCode == 200)
                return Ok(result);
            
            return BadRequest(result);
        }
    }
}
