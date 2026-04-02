namespace ScanMyBill.Interfaces;

public interface IAppNavigation
{
    public Task GoToHistoryTabAsync(CancellationToken cancellationToken = default);
}
