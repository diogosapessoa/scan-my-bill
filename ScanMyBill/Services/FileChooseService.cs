using ScanMyBill.Enums;
using ScanMyBill.Interfaces;
using ScanMyBill.Models;

namespace ScanMyBill.Services;

public sealed class FileChooseService : IFileChoose
{
    private static PickOptions _options = new();

    private static Dictionary<DevicePlatform, IEnumerable<string>> _pdfFilterPerPlatform = new()
    {
        { DevicePlatform.iOS, new[] { "" } }, // UTType values
        { DevicePlatform.Android, new[] { "application/pdf" } }, // MIME type
        { DevicePlatform.WinUI, new[] { ".pdf" } }, // file extension
        { DevicePlatform.macOS, new[] { "pdf" } }, // UTType values
    };

    private static Dictionary<DevicePlatform, IEnumerable<string>> _pngFilterPerPlatform = new()
    {
        { DevicePlatform.iOS, new[] { "" } }, // UTType values
        { DevicePlatform.Android, new[] { "image/png" } }, // MIME type
        { DevicePlatform.WinUI, new[] { ".png" } }, // file extension
        { DevicePlatform.macOS, new[] { "png" } }, // UTType values
    };

    private static Dictionary<DevicePlatform, IEnumerable<string>> _jpgFilterPerPlatform = new()
    {
        { DevicePlatform.iOS, new[] { "" } }, // UTType values
        { DevicePlatform.Android, new[] { "image/jpeg" } }, // MIME type
        { DevicePlatform.WinUI, new[] { ".jpg", ".jpeg" } }, // file extension
        { DevicePlatform.macOS, new[] { "jpg", "jpeg" } }, // UTType values
    };

    public async Task<FileChooseResult> GetPdfAsync(CancellationToken cancellationToken = default)
    {
        _options.PickerTitle = "Por favor selecione um arquivo PDF";
        _options.FileTypes = new FilePickerFileType(_pdfFilterPerPlatform);

        var result = await FilePicker.Default.PickAsync(_options);
        if (result != null)
        {
            return new FileChooseResult
            {
                Format = EFileFormat.Pdf,
                Name = result.FileName,
                Stream = await result.OpenReadAsync(),
            };
        }

        return FileChooseResult.Empty;
    }

    public async Task<FileChooseResult> GetImageAsync(EFileFormat format, CancellationToken cancellationToken = default)
    {
        _options.PickerTitle = $"Por favor selecione um arquivo {format}";

        var imageFilter = format == EFileFormat.Png ? _pngFilterPerPlatform : _jpgFilterPerPlatform;
        _options.FileTypes = new FilePickerFileType(imageFilter);

        var result = await FilePicker.Default.PickAsync(_options);
        if (result != null)
        {
            return new FileChooseResult
            {
                Format = format,
                Name = result.FileName,
                Stream = await result.OpenReadAsync(),
            };
        }

        return FileChooseResult.Empty;
    }
}
