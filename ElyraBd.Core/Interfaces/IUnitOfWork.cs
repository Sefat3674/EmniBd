namespace ElyraBd.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<T> Repository<T>() where T : Common.BaseEntity;
    IProductRepository Products { get; }
    ICategoryRepository Categories { get; }
    ICartRepository Carts { get; }
    IOrderRepository Orders { get; }
    INotificationRepository Notifications { get; }
    ICouponRepository Coupons { get; }
    IUserActivityRepository UserActivities { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
