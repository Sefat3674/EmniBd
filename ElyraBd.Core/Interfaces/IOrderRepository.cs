using ElyraBd.Core.Entities;
using ElyraBd.Core.Enums;

namespace ElyraBd.Core.Interfaces;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<Order?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Order>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Order>> GetAllWithDetailsAsync(OrderStatus? status, CancellationToken cancellationToken = default);
}
