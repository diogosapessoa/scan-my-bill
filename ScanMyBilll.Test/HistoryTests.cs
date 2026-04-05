using ScanMyBill.Core.Entities;
using ScanMyBill.Core.Enums;
using ScanMyBill.Core.Models;

namespace ScanMyBilll.Test;

public class HistoryTests
{
    [Fact]
    public void Constructor_SetsDefaultValues()
    {
        var history = new History();

        Assert.Equal(0, history.Id);
        Assert.Equal(EFileFormat.Undefined, history.Format);
        Assert.Null(history.Name);
        Assert.Null(history.Value);
        Assert.True(history.CreatedAt.Kind == DateTimeKind.Utc);
    }

    [Theory]
    [InlineData(EFileFormat.Pdf, "picture_as_pdf_24dp.png")]
    [InlineData(EFileFormat.Jpg, "image_24dp.png")]
    [InlineData(EFileFormat.Png, "image_24dp.png")]
    [InlineData(EFileFormat.Undefined, "qr_code_scanner_24dp.png")]
    public void Icon_ReturnsCorrectIconForFormat(EFileFormat format, string expectedIcon)
    {
        var history = new History { Format = format };

        Assert.Equal(expectedIcon, history.Icon);
    }

    [Fact]
    public void FromFileChooseResult_SetsFormatAndName()
    {
        var fileResult = new FileChooseResult
        {
            Format = EFileFormat.Pdf,
            Name = "test.pdf",
            Stream = new MemoryStream()
        };
        var history = new History();

        history.FromFileChooseResult(fileResult);

        Assert.Equal(EFileFormat.Pdf, history.Format);
        Assert.Equal("test.pdf", history.Name);
    }

    [Fact]
    public void FromFileChooseResult_ReturnsSameInstance()
    {
        var fileResult = new FileChooseResult { Format = EFileFormat.Jpg, Name = "test.jpg" };
        var history = new History();

        var result = history.FromFileChooseResult(fileResult);

        Assert.Same(history, result);
    }

    [Fact]
    public void WithValue_SetsValue()
    {
        var history = new History();

        history.WithValue("QR_VALUE_123");

        Assert.Equal("QR_VALUE_123", history.Value);
    }

    [Fact]
    public void WithValue_ReturnsSameInstance()
    {
        var history = new History();

        var result = history.WithValue("test");

        Assert.Same(history, result);
    }

    [Fact]
    public void WithValue_AcceptsNull()
    {
        var history = new History();

        history.WithValue(null);

        Assert.Null(history.Value);
    }

    [Fact]
    public void FluentMethods_ChainCorrectly()
    {
        var fileResult = new FileChooseResult
        {
            Format = EFileFormat.Png,
            Name = "test.png",
            Stream = new MemoryStream()
        };
        var history = new History();

        history.FromFileChooseResult(fileResult).WithValue("QR_VALUE");

        Assert.Equal(EFileFormat.Png, history.Format);
        Assert.Equal("test.png", history.Name);
        Assert.Equal("QR_VALUE", history.Value);
    }

    [Fact]
    public void ToString_ReturnsExpectedFormat()
    {
        var history = new History
        {
            Id = 1,
            Format = EFileFormat.Pdf,
            Name = "test.pdf",
            Value = "QR_VALUE",
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var result = history.ToString();

        Assert.Contains("Id: 1", result);
        Assert.Contains("Format: Pdf", result);
        Assert.Contains("Name: test.pdf", result);
        Assert.Contains("Value: QR_VALUE", result);
    }
}
