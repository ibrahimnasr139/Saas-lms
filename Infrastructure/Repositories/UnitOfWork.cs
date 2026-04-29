using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Repositories
{
    internal sealed class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task BeginTransactionAsync(CancellationToken cancellationToken)
        {
            if (_transaction is null)
            {
                _transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            }
        }
        public async Task CommitTransactionAsync(CancellationToken cancellationToken)
        {
            if (_transaction is null) return;

            await SaveAsync(cancellationToken);
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
        public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
        {
            if (_transaction is null) return;

            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
        public async Task SaveAsync(CancellationToken cancellationToken)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}