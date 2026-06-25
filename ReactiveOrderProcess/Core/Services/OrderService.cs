using ReactiveOrderProcess.Constants;
using ReactiveOrderProcess.Core.Entities;
using ReactiveOrderProcess.Core.Interfaces;
using ReactiveOrderProcess.Core.Models.Requests;
using ReactiveOrderProcess.Core.Models.Responses;

namespace ReactiveOrderProcess.Core.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMessagePublisher _messagePublisher;
        public OrderService(IOrderRepository orderRepository, IMessagePublisher messagePublisher) 
        {
            _orderRepository = orderRepository;
            _messagePublisher = messagePublisher;
        }

        public async Task<OrderResponse> AddOrderAsync(OrderRequest orderRequest)
        {
            // 1. Validation
            if (string.IsNullOrWhiteSpace(orderRequest.CustomerName))
            {
                return new OrderResponse
                {
                    StatusCode = 400,
                    Message = "Customer name cannot be empty."
                };
            }

            if (orderRequest.ProductItems == null || !orderRequest.ProductItems.Any())
            {
                return new OrderResponse
                {
                    StatusCode = 400,
                    Message = "Order must contain at least one item."
                };
            }

            foreach(var item in orderRequest.ProductItems)
            {
                if (string.IsNullOrWhiteSpace(item.ProductName))
                {
                    return new OrderResponse
                    {
                        StatusCode = 400,
                        Message = "Product name cannot be empty."
                    };
                }

                if (item.Quantity <= 0)
                {
                    return new OrderResponse
                    {
                        StatusCode = 400,
                        Message = $"Quantity for product '{item.ProductName}' must be greater than zero."
                    };
                }

                if (item.Price <= 0)
                {
                    return new OrderResponse
                    {
                        StatusCode = 400,
                        Message = $"Price for product '{item.ProductName}' must be greater than zero."
                    };
                }
            }

            decimal totalAmount = orderRequest.ProductItems.Sum(item => item.Quantity * item.Price);

            var order = new Order
            {
                CustomerName = orderRequest.CustomerName,
                Status = OrderStatus.Pending.ToString(),
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                OrderItems = orderRequest.ProductItems.Select(item => new OrderItem
                {
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    Price = item.Price
                }).ToList()
            };

            var repoRes = await _orderRepository.AddAsync(order);
           
            if(repoRes.StatusCode == 200)
            {
                //Publish event
                try
                {
                    await _messagePublisher.PublishOrderCreatedAsync(order.Id, "orders/created");
                }
                catch(Exception ex)
                {
                    return new OrderResponse
                    {
                        OrderId = order.Id,
                        Status = order.Status,
                        Message = $"Order created but failed to publish event: {ex.Message}",
                        StatusCode = 200
                    };
                }
            }

            return repoRes;

        }

        public async Task<List<Order>> GetOrdersAsync()
        {
            return await _orderRepository.GetAllAsync();
        }
    }
}
