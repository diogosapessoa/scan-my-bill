namespace ScanMyBill.Core.Interfaces;

public interface IClipboardService
{
    Task SetTextAsync(string? text);
    Task<string?> GetTextAsync();
}
