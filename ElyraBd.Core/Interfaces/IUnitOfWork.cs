namespace ElyraBd.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<T> Repository<T>() where T : Common.BaseEntity;
    IProductRepository Products { get; }
    ICategoryRepository Categories { get; }
    IUserActivityRepository UserActivities { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
