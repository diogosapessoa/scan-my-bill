using SkiaSharp;

namespace ScanMyBill.Core.Interfaces;

public interface IPdf
{
    public Task<SKBitmap> ToImageAsync(Stream pdf, CancellationToken cancellationToken = default);
}
