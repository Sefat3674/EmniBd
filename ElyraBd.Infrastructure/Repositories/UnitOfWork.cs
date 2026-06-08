using System.Collections.Concurrent;
using ElyraBd.Core.Common;
using ElyraBd.Core.Interfaces;
using ElyraBd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ElyraBd.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly ConcurrentDictionary<Type, object> _repositories = new();

    public UnitOfWork(ApplicationDbContext context) => _context = context;

    public IProductRepository Products => GetCustom<IProductRepository, ProductRepository>();
    public ICategoryRepository Categories => GetCustom<ICategoryRepository, CategoryRepository>();
    public ICartRepository Carts => GetCustom<ICartRepository, CartRepository>();
    public IOrderRepository Orders => GetCustom<IOrderRepository, OrderRepository>();
    public INotificationRepository Notifications => GetCustom<INotificationRepository, NotificationRepository>();
    public ICouponRepository Coupons => GetCustom<ICouponRepository, CouponRepository>();
    public IUserActivityRepository UserActivities => GetCustom<IUserActivityRepository, UserActivityRepository>();

    public IGenericRepository<T> Repository<T>() where T : BaseEntity =>
        (IGenericRepository<T>)_repositories.GetOrAdd(typeof(T), _ =>
            Activator.CreateInstance(typeof(GenericRepository<>).MakeGenericType(typeof(T)), _context)!);

    private TInterface GetCustom<TInterface, TImpl>() where TImpl : TInterface =>
        (TInterface)_repositories.GetOrAdd(typeof(TImpl), _ => Activator.CreateInstance(typeof(TImpl), _context)!);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken);

    public void Dispose() => _context.Dispose();
}
