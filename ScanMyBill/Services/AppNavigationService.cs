using ScanMyBill.Interfaces;

namespace ScanMyBill.Services;

public sealed class AppNavigationService : IAppNavigation
{
    public async Task GoToHistoryTabAsync(CancellationToken cancellationToken = default)
    {
        await Shell.Current.GoToAsync("//TabHistoryPage");
    }
}
