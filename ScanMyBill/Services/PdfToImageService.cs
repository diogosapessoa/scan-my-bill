using ScanMyBill.Interfaces;

using SkiaSharp;

namespace ScanMyBill.Services;

public sealed class PdfToImageService : IPdf
{
    public async Task<SKBitmap> ToImageAsync(Stream? pdf, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pdf);
        return await Task.Run(() => PDFtoImage.Conversion.ToImage(pdf));
    }
}
