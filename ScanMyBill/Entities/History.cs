using ScanMyBill.Enums;
using ScanMyBill.Models;

using SQLite;

namespace ScanMyBill.Entities;

public sealed class History
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public EFileFormat Format { get; set; }
    public string? Name { get; set; }
    public string? Value { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Ignore]
    public string Icon
    {
        get
        {
            return Format switch
            {
                EFileFormat.Pdf => "picture_as_pdf_24dp.png",
                EFileFormat.Jpg => "image_24dp.png",
                EFileFormat.Png => "image_24dp.png",
                _ => "qr_code_scanner_24dp.png"
            };
        }
    }

    public History FromFileChooseResult(FileChooseResult fileChooseResult)
    {
        Format = fileChooseResult.Format;
        Name = fileChooseResult.Name;
        return this;
    }

    public History WithValue(string? value)
    {
        Value = value;
        return this;
    }

    override public string ToString()
    {
        return $"Id: {Id}, Format: {Format}, CreatedAt: {CreatedAt}, Name: {Name}, Value: {Value}";
    }
}
