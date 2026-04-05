using ScanMyBill.Core.Interfaces;

namespace ScanMyBill.Services;

public sealed class ClipboardService : IClipboardService
{
    public Task SetTextAsync(string? text) => Clipboard.Default.SetTextAsync(text);
    public Task<string?> GetTextAsync() => Clipboard.Default.GetTextAsync();
}
