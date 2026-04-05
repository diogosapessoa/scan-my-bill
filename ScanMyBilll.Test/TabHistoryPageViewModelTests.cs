using NSubstitute;

using ScanMyBill.Core.Entities;
using ScanMyBill.Core.Enums;
using ScanMyBill.Core.Interfaces;
using ScanMyBill.Core.Repositories;
using ScanMyBill.Core.ViewModels;

namespace ScanMyBilll.Test;

public class TabHistoryPageViewModelTests
{
    private readonly IHistoryRepository _historyRepository;
    private readonly IAlert _alert;
    private readonly IClipboardService _clipboard;
    private readonly TabHistoryPageViewModel _viewModel;

    public TabHistoryPageViewModelTests()
    {
        _historyRepository = Substitute.For<IHistoryRepository>();
        _alert = Substitute.For<IAlert>();
        _clipboard = Substitute.For<IClipboardService>();

        _viewModel = new TabHistoryPageViewModel(_historyRepository, _alert, _clipboard);
    }

    [Fact]
    public void Constructor_InitializesDefaultValues()
    {
        Assert.Null(_viewModel.InputSearch);
        Assert.Equal(EFileFormat.Undefined, _viewModel.FilterFormat);
        Assert.True(_viewModel.IsFilterAll);
        Assert.False(_viewModel.IsFilterPdf);
        Assert.False(_viewModel.IsFilterImage);
        Assert.Null(_viewModel.SelectedItem);
        Assert.Empty(_viewModel.HistoryItems);
    }

    [Fact]
    public async Task LodHistoryWithFilters_CallsRepositoryWithCorrectFilters()
    {
        _viewModel.InputSearch = "test";
        _viewModel.FilterFormat = EFileFormat.Pdf;
        _historyRepository.GetAllByFilters(EFileFormat.Pdf, "test").Returns(new List<History>());

        await _viewModel.LodHistoryWithFilters();

        await _historyRepository.Received(1).GetAllByFilters(EFileFormat.Pdf, "test");
    }

    [Fact]
    public async Task LodHistoryWithFilters_PopulatesHistoryItems()
    {
        var histories = new List<History>
        {
            new() { Id = 1, Name = "file1.pdf", Format = EFileFormat.Pdf },
            new() { Id = 2, Name = "file2.png", Format = EFileFormat.Png }
        };
        _historyRepository.GetAllByFilters(Arg.Any<EFileFormat>(), Arg.Any<string>()).Returns(histories);

        await _viewModel.LodHistoryWithFilters();

        Assert.Equal(2, _viewModel.HistoryItems.Count);
    }

    [Fact]
    public async Task LodHistoryWithFilters_ClearsExistingItems()
    {
        _viewModel.HistoryItems.Add(new History { Id = 99, Name = "old" });
        _historyRepository.GetAllByFilters(Arg.Any<EFileFormat>(), Arg.Any<string>()).Returns(new List<History>());

        await _viewModel.LodHistoryWithFilters();

        Assert.Empty(_viewModel.HistoryItems);
    }

    [Fact]
    public async Task SearchEnterAsync_ReloadsWithCurrentFilters()
    {
        _viewModel.InputSearch = "search_term";
        _viewModel.FilterFormat = EFileFormat.Undefined;
        _historyRepository.GetAllByFilters(EFileFormat.Undefined, "search_term").Returns(new List<History>());

        await _viewModel.SearchEnterAsync();

        await _historyRepository.Received(1).GetAllByFilters(EFileFormat.Undefined, "search_term");
    }

    [Fact]
    public async Task FilterAllSelectedAsync_SetsCorrectFilterState()
    {
        _viewModel.IsFilterAll = false;
        _viewModel.IsFilterPdf = true;
        _viewModel.IsFilterImage = false;
        _viewModel.FilterFormat = EFileFormat.Pdf;
        _historyRepository.GetAllByFilters(EFileFormat.Undefined, null).Returns(new List<History>());

        await _viewModel.FilterAllSelectedAsync();

        Assert.True(_viewModel.IsFilterAll);
        Assert.False(_viewModel.IsFilterPdf);
        Assert.False(_viewModel.IsFilterImage);
        Assert.Equal(EFileFormat.Undefined, _viewModel.FilterFormat);
    }

    [Fact]
    public async Task FilterPdfSelectedAsync_SetsCorrectFilterState()
    {
        _viewModel.IsFilterAll = true;
        _viewModel.IsFilterPdf = false;
        _viewModel.IsFilterImage = false;
        _historyRepository.GetAllByFilters(EFileFormat.Pdf, null).Returns(new List<History>());

        await _viewModel.FilterPdfSelectedAsync();

        Assert.False(_viewModel.IsFilterAll);
        Assert.True(_viewModel.IsFilterPdf);
        Assert.False(_viewModel.IsFilterImage);
        Assert.Equal(EFileFormat.Pdf, _viewModel.FilterFormat);
    }

    [Fact]
    public async Task FilterImageSelectedAsync_SetsCorrectFilterState()
    {
        _viewModel.IsFilterAll = true;
        _viewModel.IsFilterPdf = false;
        _viewModel.IsFilterImage = false;
        _historyRepository.GetAllByFilters(EFileFormat.Png, null).Returns(new List<History>());

        await _viewModel.FilterImageSelectedAsync();

        Assert.False(_viewModel.IsFilterAll);
        Assert.False(_viewModel.IsFilterPdf);
        Assert.True(_viewModel.IsFilterImage);
        Assert.Equal(EFileFormat.Png, _viewModel.FilterFormat);
    }

    [Fact]
    public async Task HistorySelectedItemAsync_WhenSelectedItemIsNull_DoesNothing()
    {
        _viewModel.SelectedItem = null;

        await _viewModel.HistorySelectedItemAsync();

        await _alert.DidNotReceive().AcceptAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        await _alert.DidNotReceive().ShowActionAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task HistorySelectedItemAsync_WhenValueIsEmptyOrNull_ShowsDeleteConfirmation()
    {
        _viewModel.SelectedItem = new History { Id = 1, Name = "empty.pdf", Value = null };
        _alert.AcceptAsync("Deseja deletar?", "empty.pdf", "Sim", "Não").Returns(true);

        await _viewModel.HistorySelectedItemAsync();

        await _historyRepository.Received(1).DeleteByIdAsync(1);
        Assert.Empty(_viewModel.HistoryItems);
        Assert.Null(_viewModel.SelectedItem);
    }

    [Fact]
    public async Task HistorySelectedItemAsync_WhenValueIsEmptyAndUserDeclines_DoesNotDelete()
    {
        _viewModel.HistoryItems.Add(new History { Id = 1, Name = "empty.pdf", Value = null });
        _viewModel.SelectedItem = _viewModel.HistoryItems[0];
        _alert.AcceptAsync("Deseja deletar?", "empty.pdf", "Sim", "Não").Returns(false);

        await _viewModel.HistorySelectedItemAsync();

        await _historyRepository.DidNotReceive().DeleteByIdAsync(Arg.Any<int>());
        Assert.Single(_viewModel.HistoryItems);
    }

    [Fact]
    public async Task HistorySelectedItemAsync_WhenValueExists_ShowsActionDialog()
    {
        _viewModel.HistoryItems.Add(new History { Id = 1, Name = "file.pdf", Value = "QR_VALUE" });
        _viewModel.SelectedItem = _viewModel.HistoryItems[0];
        _alert.ShowActionAsync("file.pdf", "Cancelar", "Deletar", "Copiar para área de transferência").Returns("Deletar");

        await _viewModel.HistorySelectedItemAsync();

        await _historyRepository.Received(1).DeleteByIdAsync(1);
        Assert.Empty(_viewModel.HistoryItems);
    }

    [Fact]
    public async Task HistorySelectedItemAsync_WhenUserChoosesCopy_CopiesToClipboard()
    {
        _viewModel.HistoryItems.Add(new History { Id = 1, Name = "file.pdf", Value = "QR_VALUE" });
        _viewModel.SelectedItem = _viewModel.HistoryItems[0];
        _alert.ShowActionAsync("file.pdf", "Cancelar", "Deletar", "Copiar para área de transferência").Returns("Copiar para área de transferência");

        await _viewModel.HistorySelectedItemAsync();

        await _clipboard.Received(1).SetTextAsync("QR_VALUE");
        await _historyRepository.DidNotReceive().DeleteByIdAsync(Arg.Any<int>());
    }

    [Fact]
    public async Task HistorySelectedItemAsync_WhenUserCancels_DoesNothing()
    {
        _viewModel.HistoryItems.Add(new History { Id = 1, Name = "file.pdf", Value = "QR_VALUE" });
        _viewModel.SelectedItem = _viewModel.HistoryItems[0];
        _alert.ShowActionAsync("file.pdf", "Cancelar", "Deletar", "Copiar para área de transferência").Returns("");

        await _viewModel.HistorySelectedItemAsync();

        await _clipboard.DidNotReceive().SetTextAsync(Arg.Any<string>());
        await _historyRepository.DidNotReceive().DeleteByIdAsync(Arg.Any<int>());
    }

    [Fact]
    public async Task HistorySelectedItemAsync_ClearsSelection()
    {
        _viewModel.HistoryItems.Add(new History { Id = 1, Name = "file.pdf", Value = "QR_VALUE" });
        _viewModel.SelectedItem = _viewModel.HistoryItems[0];
        _alert.ShowActionAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns("Cancelar");

        await _viewModel.HistorySelectedItemAsync();

        Assert.Null(_viewModel.SelectedItem);
    }
}
