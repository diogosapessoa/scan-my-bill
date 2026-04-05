using ScanMyBill.Core.Interfaces;

namespace ScanMyBill.Services;

public sealed class AlertService : IAlert
{
    public async Task ShowAsync(string? title, string? message, CancellationToken cancellationToken = default)
    {
        var page = App.Current?.Windows[0]?.Page;
        if (page == null)
            return;
        await page.DisplayAlertAsync(title, message, "OK");
    }

    public async Task<bool> AcceptAsync(string? title, string? message, string? accept, string? cancel, CancellationToken cancellationToken = default)
    {
        var page = App.Current?.Windows[0]?.Page;
        if (page == null)
            return false;
        return await page.DisplayAlertAsync(title, message, accept, cancel);
    }

    public async Task<string> ShowActionAsync(string? title, string? cancel, string? principalAction, string? secondaryAction)
    {
        var page = App.Current?.Windows[0]?.Page;
        if (page == null)
            return string.Empty;
        return await page.DisplayActionSheetAsync(title, cancel, principalAction, secondaryAction);
    }
}
