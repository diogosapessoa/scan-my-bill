using ScanMyBill.Core.Entities;
using ScanMyBill.Core.Enums;

namespace ScanMyBill.Core.Repositories;

public interface IHistoryRepository
{
    public Task<List<History>> GetAllAsync(CancellationToken cancellationToken = default);
    public Task<List<History>> GetAllByFilters(EFileFormat format, string? name, CancellationToken cancellationToken = default);
    public Task<List<History>> GetRecents(int limit, CancellationToken cancellationToken = default);

    public Task<History?> FindByIdAsync(int id, CancellationToken cancellationToken = default);

    public Task SaveAsync(History history, CancellationToken cancellationToken = default);

    public Task DeleteByIdAsync(int id, CancellationToken cancellationToken = default);
}
