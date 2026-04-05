namespace ScanMyBill.Core.Interfaces;

public interface IAppNavigation
{
    public Task GoToHistoryTabAsync(CancellationToken cancellationToken = default);
}
