using ScanMyBill.Core.Interfaces;

using SkiaSharp;

namespace ScanMyBill.Services;

public sealed class QrCodeService : IQrCode
{
    public async Task<string?> ScanAsync(SKBitmap image, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => ReadQrCode(image));
    }

    public static string? ReadQrCode(SKBitmap image)
    {
        var reader = new ZXing.SkiaSharp.BarcodeReader();

        var result = reader.Decode(image);

        return result?.Text;
    }
}
