using ScanMyBill.Core.Converters;

namespace ScanMyBilll.Test;

public class UtcToLocalConverterTests
{
    private readonly UtcToLocalConverter _converter;

    public UtcToLocalConverterTests()
    {
        _converter = new UtcToLocalConverter();
    }

    [Fact]
    public void Convert_WhenValueIsDateTime_ConvertsToLocalTime()
    {
        var utcTime = new DateTime(2024, 6, 15, 12, 0, 0, DateTimeKind.Utc);

        var result = _converter.Convert(utcTime, typeof(string), null, System.Globalization.CultureInfo.CurrentCulture);

        Assert.NotNull(result);
        var localTime = Assert.IsType<DateTime>(result);
        Assert.Equal(utcTime.ToLocalTime(), localTime);
    }

    [Fact]
    public void Convert_WhenValueIsUnspecifiedKind_TreatsAsUtc()
    {
        var unspecifiedTime = new DateTime(2024, 6, 15, 12, 0, 0, DateTimeKind.Unspecified);

        var result = _converter.Convert(unspecifiedTime, typeof(string), null, System.Globalization.CultureInfo.CurrentCulture);

        Assert.NotNull(result);
        var localTime = Assert.IsType<DateTime>(result);
        Assert.Equal(DateTime.SpecifyKind(unspecifiedTime, DateTimeKind.Utc).ToLocalTime(), localTime);
    }

    [Fact]
    public void Convert_WhenValueIsAlreadyUtc_ReturnsCorrectLocalTime()
    {
        var utcTime = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var result = _converter.Convert(utcTime, typeof(string), null, System.Globalization.CultureInfo.CurrentCulture);

        Assert.NotNull(result);
        var localTime = Assert.IsType<DateTime>(result);
        Assert.Equal(DateTimeKind.Local, localTime.Kind);
    }

    [Fact]
    public void Convert_WhenValueIsNull_ReturnsNull()
    {
        var result = _converter.Convert(null, typeof(string), null, System.Globalization.CultureInfo.CurrentCulture);

        Assert.Null(result);
    }

    [Fact]
    public void Convert_WhenValueIsNotDateTime_ReturnsOriginalValue()
    {
        var stringValue = "not a date";

        var result = _converter.Convert(stringValue, typeof(string), null, System.Globalization.CultureInfo.CurrentCulture);

        Assert.Equal(stringValue, result);
    }

    [Fact]
    public void Convert_WhenValueIsInt_ReturnsOriginalValue()
    {
        var intValue = 42;

        var result = _converter.Convert(intValue, typeof(string), null, System.Globalization.CultureInfo.CurrentCulture);

        Assert.Equal(intValue, result);
    }

    [Fact]
    public void ConvertBack_AlwaysThrowsNotImplementedException()
    {
        Assert.Throws<NotImplementedException>(() =>
            _converter.ConvertBack(DateTime.Now, typeof(DateTime), null, System.Globalization.CultureInfo.CurrentCulture));
    }

    [Fact]
    public void Convert_PreservesTimezoneOffset()
    {
        var utcTime = new DateTime(2024, 6, 15, 12, 0, 0, DateTimeKind.Utc);
        var expectedLocal = utcTime.ToLocalTime();

        var result = _converter.Convert(utcTime, typeof(DateTime), null, System.Globalization.CultureInfo.CurrentCulture);

        Assert.Equal(expectedLocal, result);
    }
}
