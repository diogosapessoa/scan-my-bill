namespace ScanMyBill.Interfaces;

public interface IAlert
{
    public Task ShowAsync(string? title, string? message, CancellationToken cancellationToken = default);
    public Task<bool> AcceptAsync(string? title, string? message, string? accept, string? cancel, CancellationToken cancellationToken = default);
    public Task<string> ShowActionAsync(string? title, string? cancel, string? principalAction, string? secondaryAction);
}
