using SkiaSharp;

namespace ScanMyBill.Interfaces;

public interface IQrCode
{
    public Task<string?> ScanAsync(SKBitmap image, CancellationToken cancellationToken = default);
}
