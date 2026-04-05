using ScanMyBill.Core.Enums;

namespace ScanMyBill.Core.Models;

public sealed class FileChooseResult : IDisposable
{
    public static FileChooseResult Empty => new() { Format = EFileFormat.Undefined, Name = null, Stream = null };

    public EFileFormat Format { get; set; }
    public string? Name { get; set; }
    public Stream? Stream { get; set; }

    public bool HasStream => Stream != null && Stream != Stream.Null;

    override public string ToString()
    {
        return $"Format: {Format}, Name: {Name}";
    }

    public void Dispose()
    {
        Stream?.Dispose();
    }
}
