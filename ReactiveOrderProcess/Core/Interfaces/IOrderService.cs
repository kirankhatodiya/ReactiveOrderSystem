using ReactiveOrderProcess.Core.Entities;
using ReactiveOrderProcess.Core.Models.Requests;
using ReactiveOrderProcess.Core.Models.Responses;

namespace ReactiveOrderProcess.Core.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponse> AddOrderAsync(OrderRequest orderRequest);
        Task<List<Order>> GetOrdersAsync();
    }
}
