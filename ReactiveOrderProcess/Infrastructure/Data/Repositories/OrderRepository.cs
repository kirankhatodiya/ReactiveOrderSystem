using Microsoft.EntityFrameworkCore;
using ReactiveOrderProcess.Constants;
using ReactiveOrderProcess.Core.Entities;
using ReactiveOrderProcess.Core.Interfaces;
using ReactiveOrderProcess.Core.Models.Requests;
using ReactiveOrderProcess.Core.Models.Responses;

namespace ReactiveOrderProcess.Infrastructure.Data.Repositories
{
    public class OrderRepository:IOrderRepository
    {
        private readonly OrderDbContext _context;
        private readonly DbSet<Order> _orders;
        public OrderRepository(OrderDbContext context)
        {
            _context = context;
            _orders = _context.Set<Order>();
        }
        public async Task<OrderResponse> AddAsync(Order order)
        {
            await _orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return new OrderResponse
            {
                OrderId = order.Id,
                StatusCode = 200,
                Status = order.Status,
                Message = "Order added successfully!"
            };
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _orders.Include(o => o.OrderItems).ToListAsync();
        }

        public async Task<Order> GetByIdAsync(int OrderId)
        {
            return await _orders.Where(o => o.Id == OrderId).Include(o => o.OrderItems).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(Order order)
        {
            _orders.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}
