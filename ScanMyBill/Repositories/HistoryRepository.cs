using ScanMyBill.Data;
using ScanMyBill.Entities;
using ScanMyBill.Enums;

using SQLite;

using System.Linq.Expressions;

namespace ScanMyBill.Repositories;

public sealed class HistoryRepository : IHistoryRepository
{
    public const int MaxHistoryItems = 100; // Limite fixado para evitar sobrecarga de memória, pode ser ajustado conforme necessário

    private readonly SQLiteAsyncConnection _connection;

    public HistoryRepository()
    {
        _connection = DatabaseHelper.Connection;
    }

    public async Task<List<History>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await _connection.CreateTableAsync<History>();

        return await _connection.Table<History>().Take(MaxHistoryItems).ToListAsync();
    }

    public async Task<List<History>> GetAllByFilters(EFileFormat format, string? name, CancellationToken cancellationToken = default)
    {
        await _connection.CreateTableAsync<History>();

        Expression<Func<History, bool>> filter;

        if (format == EFileFormat.Undefined && !string.IsNullOrWhiteSpace(name))
        {
            filter = h => h.Name != null && h.Name.Contains(name);
        }
        else if (format != EFileFormat.Undefined && string.IsNullOrWhiteSpace(name))
        {
            if (format == EFileFormat.Pdf)
                filter = h => h.Format == format;
            else
                filter = h => h.Format == EFileFormat.Png || h.Format == EFileFormat.Jpg;
        }
        else if (format != EFileFormat.Undefined && !string.IsNullOrWhiteSpace(name))
        {
            if (format == EFileFormat.Pdf)
                filter = h => h.Format == format && h.Name != null && h.Name.Contains(name);
            else
                filter = h => (h.Format == EFileFormat.Png || h.Format == EFileFormat.Jpg) && h.Name != null && h.Name.Contains(name);
        }
        else
            filter = h => true; // Sem filtros, retorna tudo

        return await _connection.Table<History>()
            .OrderByDescending(x => x.Id)
            .Take(MaxHistoryItems)
            .Where(filter)
            .ToListAsync();
    }

    public async Task<List<History>> GetRecents(int limit, CancellationToken cancellationToken = default)
    {
        await _connection.CreateTableAsync<History>();

        return await _connection.Table<History>()
            .OrderByDescending(x => x.Id)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<History?> FindByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _connection.FindAsync<History>(id);
    }

    public async Task SaveAsync(History history, CancellationToken cancellationToken = default)
    {
        await _connection.InsertAsync(history);
    }

    public async Task DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await _connection.DeleteAsync<History>(id);
    }
}
