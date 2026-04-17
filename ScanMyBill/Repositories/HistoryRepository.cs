using ScanMyBill.Data;
using ScanMyBill.Core.Entities;
using ScanMyBill.Core.Enums;
using ScanMyBill.Core.Repositories;

using System.Linq.Expressions;

namespace ScanMyBill.Repositories;

public sealed class HistoryRepository : IHistoryRepository
{
    public const int MaxHistoryItems = 100; // Limite fixado para evitar sobrecarga de memória, pode ser ajustado conforme necessário

    private readonly IDatabaseHelper _database;

    public HistoryRepository(IDatabaseHelper databaseHelper)
    {
        _database = databaseHelper;
    }

    public async Task<List<History>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await _database.Connection.CreateTableAsync<History>();

        return await _database.Connection.Table<History>().Take(MaxHistoryItems).ToListAsync();
    }

    public async Task<List<History>> GetAllByFilters(EFileFormat format, string? name, CancellationToken cancellationToken = default)
    {
        await _database.Connection.CreateTableAsync<History>();
        Expression<Func<History, bool>> filter = GenerateSearchFilter(format, name);

        return await _database.Connection.Table<History>()
            .OrderByDescending(x => x.Id)
            .Take(MaxHistoryItems)
            .Where(filter)
            .ToListAsync();
    }

    private static Expression<Func<History, bool>> GenerateSearchFilter(EFileFormat format, string? name)
    {
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

        return filter;
    }

    public async Task<List<History>> GetRecents(int limit, CancellationToken cancellationToken = default)
    {
        await _database.Connection.CreateTableAsync<History>();

        return await _database.Connection.Table<History>()
            .OrderByDescending(x => x.Id)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<History?> FindByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _database.Connection.FindAsync<History>(id);
    }

    public async Task SaveAsync(History history, CancellationToken cancellationToken = default)
    {
        await _database.Connection.InsertAsync(history);
    }

    public async Task DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await _database.Connection.DeleteAsync<History>(id);
    }
}
