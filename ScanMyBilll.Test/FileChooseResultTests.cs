using ScanMyBill.Core.Enums;
using ScanMyBill.Core.Models;

namespace ScanMyBilll.Test;

public class FileChooseResultTests
{
    [Fact]
    public void Empty_ReturnsInstanceWithUndefinedFormat()
    {
        var result = FileChooseResult.Empty;

        Assert.Equal(EFileFormat.Undefined, result.Format);
        Assert.Null(result.Name);
        Assert.Null(result.Stream);
    }

    [Fact]
    public void Empty_ReturnsNewInstanceEachTime()
    {
        var result1 = FileChooseResult.Empty;
        var result2 = FileChooseResult.Empty;

        Assert.NotSame(result1, result2);
    }

    [Fact]
    public void HasStream_ReturnsTrue_WhenStreamIsValid()
    {
        var result = new FileChooseResult
        {
            Stream = new MemoryStream()
        };

        Assert.True(result.HasStream);
    }

    [Fact]
    public void HasStream_ReturnsFalse_WhenStreamIsNull()
    {
        var result = new FileChooseResult
        {
            Stream = null
        };

        Assert.False(result.HasStream);
    }

    [Fact]
    public void HasStream_ReturnsFalse_WhenStreamIsNullStream()
    {
        var result = new FileChooseResult
        {
            Stream = Stream.Null
        };

        Assert.False(result.HasStream);
    }

    [Fact]
    public void Dispose_DisposesStream()
    {
        var stream = new MemoryStream();
        var result = new FileChooseResult
        {
            Stream = stream
        };

        result.Dispose();

        Assert.False(stream.CanRead);
    }

    [Fact]
    public void Dispose_DoesNotThrow_WhenStreamIsNull()
    {
        var result = new FileChooseResult
        {
            Stream = null
        };

        var exception = Record.Exception(() => result.Dispose());

        Assert.Null(exception);
    }

    [Fact]
    public void ToString_ReturnsExpectedFormat()
    {
        var result = new FileChooseResult
        {
            Format = EFileFormat.Pdf,
            Name = "test.pdf"
        };

        var output = result.ToString();

        Assert.Equal("Format: Pdf, Name: test.pdf", output);
    }

    [Fact]
    public void ToString_HandlesNullName()
    {
        var result = new FileChooseResult
        {
            Format = EFileFormat.Png,
            Name = null
        };

        var output = result.ToString();

        Assert.Equal("Format: Png, Name: ", output);
    }

    [Fact]
    public void Properties_CanBeSetAndRead()
    {
        var stream = new MemoryStream();
        var result = new FileChooseResult
        {
            Format = EFileFormat.Jpg,
            Name = "photo.jpg",
            Stream = stream
        };

        Assert.Equal(EFileFormat.Jpg, result.Format);
        Assert.Equal("photo.jpg", result.Name);
        Assert.Same(stream, result.Stream);
    }
}
