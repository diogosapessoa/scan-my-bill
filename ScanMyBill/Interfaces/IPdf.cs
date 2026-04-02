using SkiaSharp;

namespace ScanMyBill.Interfaces;

public interface IPdf
{
    public Task<SKBitmap> ToImageAsync(Stream pdf, CancellationToken cancellationToken = default);
}
