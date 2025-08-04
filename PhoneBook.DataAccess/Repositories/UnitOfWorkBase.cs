using Microsoft.EntityFrameworkCore;
using PhoneBook.Contracts.Repositories;

namespace PhoneBook.DataAccess.Repositories;

public class UnitOfWorkBase(DbContext db) : IUnitOfWorkBase
{
    private readonly DbContext _db = db ?? throw new ArgumentNullException(nameof(db));

    private bool _disposed;

    public async Task SaveAsync()
    {
        ObjectDisposedException.ThrowIf(_disposed, db);

        if (_db.Database.CurrentTransaction != null)
        {
            await _db.Database.CommitTransactionAsync();
        }

        await _db.SaveChangesAsync();
    }

    public Task BeginTransactionAsync()
    {
        ObjectDisposedException.ThrowIf(_disposed, db);
        return _db.Database.BeginTransactionAsync();
    }

    public Task RollbackTransactionAsync()
    {
        ObjectDisposedException.ThrowIf(_disposed, db);
        return _db.Database.RollbackTransactionAsync();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        if (_db.Database.CurrentTransaction != null)
        {
            _db.Database.RollbackTransaction();
        }

        _db.Dispose();
        _disposed = true;
    }
}
