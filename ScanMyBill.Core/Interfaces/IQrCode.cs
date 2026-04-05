using SkiaSharp;

namespace ScanMyBill.Core.Interfaces;

public interface IQrCode
{
    public Task<string?> ScanAsync(SKBitmap image, CancellationToken cancellationToken = default);
}
