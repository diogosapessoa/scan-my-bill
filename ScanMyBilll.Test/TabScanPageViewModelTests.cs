using NSubstitute;

using ScanMyBill.Core.Entities;
using ScanMyBill.Core.Enums;
using ScanMyBill.Core.Interfaces;
using ScanMyBill.Core.Models;
using ScanMyBill.Core.Repositories;
using ScanMyBill.Core.ViewModels;

using SkiaSharp;

namespace ScanMyBilll.Test;

public class TabScanPageViewModelTests
{
    private readonly IFileChoose _fileChoose;
    private readonly IPdf _pdf;
    private readonly IQrCode _qrCode;
    private readonly IAlert _alert;
    private readonly IClipboardService _clipboard;
    private readonly IHistoryRepository _historyRepository;
    private readonly IAppNavigation _navigation;
    private readonly TabScanPageViewModel _viewModel;

    public TabScanPageViewModelTests()
    {
        _fileChoose = Substitute.For<IFileChoose>();
        _pdf = Substitute.For<IPdf>();
        _qrCode = Substitute.For<IQrCode>();
        _alert = Substitute.For<IAlert>();
        _clipboard = Substitute.For<IClipboardService>();
        _historyRepository = Substitute.For<IHistoryRepository>();
        _navigation = Substitute.For<IAppNavigation>();

        _viewModel = new TabScanPageViewModel(
            _fileChoose, _pdf, _qrCode, _alert, _clipboard, _historyRepository, _navigation);
    }

    [Fact]
    public void Constructor_InitializesEmptyHistoryItems()
    {
        Assert.Empty(_viewModel.HistoryItems);
    }

    [Fact]
    public void Constructor_SetsSelectedItemToNull()
    {
        Assert.Null(_viewModel.SelectedItem);
    }

    [Fact]
    public async Task SelectPdfAsync_WhenFileChosenIsNull_DoesNothing()
    {
        _fileChoose.GetPdfAsync().Returns((FileChooseResult?)null);

        await _viewModel.SelectPdfAsync();

        await _historyRepository.DidNotReceive().SaveAsync(Arg.Any<History>());
        Assert.Empty(_viewModel.HistoryItems);
    }

    [Fact]
    public async Task SelectPdfAsync_WhenStreamIsNull_DoesNothing()
    {
        _fileChoose.GetPdfAsync().Returns(new FileChooseResult { Stream = null });

        await _viewModel.SelectPdfAsync();

        await _historyRepository.DidNotReceive().SaveAsync(Arg.Any<History>());
        Assert.Empty(_viewModel.HistoryItems);
    }

    [Fact]
    public async Task SelectPdfAsync_WhenQrCodeNotFound_ShowsWarning()
    {
        var pdfStream = new MemoryStream();
        var pdfResult = new FileChooseResult { Format = EFileFormat.Pdf, Name = "test.pdf", Stream = pdfStream };
        _fileChoose.GetPdfAsync().Returns(pdfResult);
        var bitmap = new SKBitmap(1, 1);
        _pdf.ToImageAsync(pdfStream).Returns(bitmap);
        _qrCode.ScanAsync(Arg.Any<SKBitmap>()).Returns((string?)null);

        await _viewModel.SelectPdfAsync();

        await _alert.Received(1).ShowAsync("Aviso", "Não foi encontrado QRCODE no arquivo.");
        await _historyRepository.DidNotReceive().SaveAsync(Arg.Any<History>());
        Assert.Empty(_viewModel.HistoryItems);
    }

    [Fact]
    public async Task SelectPdfAsync_WhenQrCodeFound_SavesAndShowsResult()
    {
        var pdfStream = new MemoryStream();
        var pdfResult = new FileChooseResult { Format = EFileFormat.Pdf, Name = "test.pdf", Stream = pdfStream };
        _fileChoose.GetPdfAsync().Returns(pdfResult);
        var bitmap = new SKBitmap(1, 1);
        _pdf.ToImageAsync(pdfStream).Returns(bitmap);
        _qrCode.ScanAsync(Arg.Any<SKBitmap>()).Returns("QR_VALUE_123");
        _alert.AcceptAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(true);

        await _viewModel.SelectPdfAsync();

        await _historyRepository.Received(1).SaveAsync(Arg.Is<History>(h =>
            h.Format == EFileFormat.Pdf && h.Name == "test.pdf" && h.Value == "QR_VALUE_123"));
        Assert.Single(_viewModel.HistoryItems);
        Assert.Equal("test.pdf", _viewModel.HistoryItems[0].Name);
        await _clipboard.Received(1).SetTextAsync("QR_VALUE_123");
    }

    [Fact]
    public async Task SelectJpgAsync_WhenFileChosenIsNull_DoesNothing()
    {
        _fileChoose.GetImageAsync(EFileFormat.Jpg).Returns((FileChooseResult?)null);

        await _viewModel.SelectJpgAsync();

        await _historyRepository.DidNotReceive().SaveAsync(Arg.Any<History>());
        Assert.Empty(_viewModel.HistoryItems);
    }

    [Fact]
    public async Task SelectJpgAsync_WhenHasStreamIsFalse_DoesNothing()
    {
        _fileChoose.GetImageAsync(EFileFormat.Jpg).Returns(new FileChooseResult { Format = EFileFormat.Jpg, Stream = null });

        await _viewModel.SelectJpgAsync();

        await _historyRepository.DidNotReceive().SaveAsync(Arg.Any<History>());
        Assert.Empty(_viewModel.HistoryItems);
    }

    [Fact]
    public async Task SelectJpgAsync_WhenQrCodeFound_SavesAndShowsResult()
    {
        var jpgStream = new MemoryStream();
        var jpgResult = new FileChooseResult { Format = EFileFormat.Jpg, Name = "test.jpg", Stream = jpgStream };
        _fileChoose.GetImageAsync(EFileFormat.Jpg).Returns(jpgResult);
        _qrCode.ScanAsync(Arg.Any<SKBitmap>()).Returns("QR_JPG_VALUE");
        _alert.AcceptAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(true);

        await _viewModel.SelectJpgAsync();

        await _historyRepository.Received(1).SaveAsync(Arg.Is<History>(h =>
            h.Format == EFileFormat.Jpg && h.Name == "test.jpg" && h.Value == "QR_JPG_VALUE"));
        Assert.Single(_viewModel.HistoryItems);
    }

    [Fact]
    public async Task SelectPngAsync_WhenFileChosenIsNull_DoesNothing()
    {
        _fileChoose.GetImageAsync(EFileFormat.Png).Returns((FileChooseResult?)null);

        await _viewModel.SelectPngAsync();

        await _historyRepository.DidNotReceive().SaveAsync(Arg.Any<History>());
        Assert.Empty(_viewModel.HistoryItems);
    }

    [Fact]
    public async Task SelectPngAsync_WhenQrCodeFound_SavesAndShowsResult()
    {
        var pngStream = new MemoryStream();
        var pngResult = new FileChooseResult { Format = EFileFormat.Png, Name = "test.png", Stream = pngStream };
        _fileChoose.GetImageAsync(EFileFormat.Png).Returns(pngResult);
        _qrCode.ScanAsync(Arg.Any<SKBitmap>()).Returns("QR_PNG_VALUE");
        _alert.AcceptAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(true);

        await _viewModel.SelectPngAsync();

        await _historyRepository.Received(1).SaveAsync(Arg.Is<History>(h =>
            h.Format == EFileFormat.Png && h.Name == "test.png" && h.Value == "QR_PNG_VALUE"));
        Assert.Single(_viewModel.HistoryItems);
    }

    [Fact]
    public async Task LoadRecentsAsync_LoadsHistoryFromRepository()
    {
        var histories = new List<History>
        {
            new() { Id = 1, Name = "file1.pdf", Format = EFileFormat.Pdf, Value = "value1" },
            new() { Id = 2, Name = "file2.png", Format = EFileFormat.Png, Value = "value2" }
        };
        _historyRepository.GetRecents(TabScanPageViewModel.MaxHistoryItems).Returns(histories);

        await _viewModel.LoadRecentsAsync();

        Assert.Equal(2, _viewModel.HistoryItems.Count);
        Assert.Equal("file1.pdf", _viewModel.HistoryItems[0].Name);
        Assert.Equal("file2.png", _viewModel.HistoryItems[1].Name);
    }

    [Fact]
    public async Task LoadRecentsAsync_ClearsExistingItems()
    {
        _viewModel.HistoryItems.Add(new History { Id = 99, Name = "old" });
        _historyRepository.GetRecents(TabScanPageViewModel.MaxHistoryItems).Returns(new List<History>());

        await _viewModel.LoadRecentsAsync();

        Assert.Empty(_viewModel.HistoryItems);
    }

    [Fact]
    public async Task RecentSelectedItemAsync_WhenSelectedItemIsNull_DoesNothing()
    {
        _viewModel.SelectedItem = null;

        await _viewModel.RecentSelectedItemAsync();

        await _alert.DidNotReceive().AcceptAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task RecentSelectedItemAsync_WhenValueIsEmpty_DoesNothing()
    {
        _viewModel.SelectedItem = new History { Id = 1, Value = "" };

        await _viewModel.RecentSelectedItemAsync();

        await _alert.DidNotReceive().AcceptAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task RecentSelectedItemAsync_WhenUserAccepts_CopiesToClipboard()
    {
        _viewModel.SelectedItem = new History { Id = 1, Value = "QR_VALUE" };
        _alert.AcceptAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(true);

        await _viewModel.RecentSelectedItemAsync();

        await _clipboard.Received(1).SetTextAsync("QR_VALUE");
        Assert.Null(_viewModel.SelectedItem);
    }

    [Fact]
    public async Task RecentSelectedItemAsync_WhenUserDeclines_DoesNotCopy()
    {
        _viewModel.SelectedItem = new History { Id = 1, Value = "QR_VALUE" };
        _alert.AcceptAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(false);

        await _viewModel.RecentSelectedItemAsync();

        await _clipboard.DidNotReceive().SetTextAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task GoToHistoryTabAsync_CallsNavigation()
    {
        await _viewModel.GoToHistoryTabAsync();

        await _navigation.Received(1).GoToHistoryTabAsync();
    }

    [Fact]
    public async Task SelectPdfAsync_InsertsNewItemAtIndexZero()
    {
        var pdfStream = new MemoryStream();
        var pdfResult = new FileChooseResult { Format = EFileFormat.Pdf, Name = "test.pdf", Stream = pdfStream };
        _fileChoose.GetPdfAsync().Returns(pdfResult);
        var bitmap = new SKBitmap(1, 1);
        _pdf.ToImageAsync(pdfStream).Returns(bitmap);
        _qrCode.ScanAsync(Arg.Any<SKBitmap>()).Returns("QR_VALUE");
        _alert.AcceptAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(true);

        _viewModel.HistoryItems.Add(new History { Id = 99, Name = "old" });

        await _viewModel.SelectPdfAsync();

        Assert.Equal(2, _viewModel.HistoryItems.Count);
        Assert.Equal("test.pdf", _viewModel.HistoryItems[0].Name);
    }
}
