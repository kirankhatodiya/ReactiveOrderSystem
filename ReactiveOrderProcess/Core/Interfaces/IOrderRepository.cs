using ReactiveOrderProcess.Core.Entities;
using ReactiveOrderProcess.Core.Models.Requests;
using ReactiveOrderProcess.Core.Models.Responses;

namespace ReactiveOrderProcess.Core.Interfaces
{
    public interface IOrderRepository
    {
        Task<OrderResponse> AddAsync(Order orderRequest);
        Task<List<Order>> GetAllAsync();
        Task<Order> GetByIdAsync(int orderId);
        Task UpdateAsync(Order order);
    }
}
