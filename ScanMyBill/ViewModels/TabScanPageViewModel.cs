using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ScanMyBill.Entities;
using ScanMyBill.Enums;
using ScanMyBill.Interfaces;
using ScanMyBill.Repositories;

using SkiaSharp;

using System.Collections.ObjectModel;

namespace ScanMyBill.ViewModels;

public partial class TabScanPageViewModel : ObservableObject
{
    [ObservableProperty]
    private History? selectedItem;

    public ObservableCollection<History> HistoryItems { get; set; } = new();

    private readonly IFileChoose _fileChoose;
    private readonly IPdf _pdf;
    private readonly IQrCode _qrCode;
    private readonly IAlert _alert;
    private readonly IClipboard _clipboard;
    private readonly IHistoryRepository _historyRepository;
    private readonly IAppNavigation _navigation;

    public TabScanPageViewModel(IFileChoose fileChoose, IPdf pdf, IQrCode qrCode,
        IAlert alert, IClipboard clipboard, IHistoryRepository historyRepository, IAppNavigation navigation)
    {
        _fileChoose = fileChoose;
        _pdf = pdf;
        _qrCode = qrCode;
        _alert = alert;
        _clipboard = clipboard;
        _historyRepository = historyRepository;
        _navigation = navigation;
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task SelectPdfAsync(CancellationToken cancellationToken = default)
    {
        using var pdfResult = await _fileChoose.GetPdfAsync(cancellationToken);
        if (pdfResult == null || !pdfResult.HasStream) return;
        using var skBitmap = await _pdf.ToImageAsync(pdfResult.Stream);
        string? value = await ScanAndShowQrCode(skBitmap);
        if (string.IsNullOrWhiteSpace(value))
        {
            await _alert.ShowAsync("Aviso", "Não foi encontrado QRCODE no arquivo.");
            return;
        }
        var history = new History().FromFileChooseResult(pdfResult).WithValue(value);
        await _historyRepository.SaveAsync(history);
        HistoryItems.Insert(0, history);
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task SelectJpgAsync(CancellationToken cancellationToken = default)
    {
        using var jpgResult = await _fileChoose.GetImageAsync(EFileFormat.Jpg, cancellationToken);
        if (jpgResult == null || !jpgResult.HasStream) return;
        using var skBitmap = SKBitmap.Decode(jpgResult.Stream);
        string? value = await ScanAndShowQrCode(skBitmap);
        if (string.IsNullOrWhiteSpace(value))
        {
            await _alert.ShowAsync("Aviso", "Não foi encontrado QRCODE no arquivo.");
            return;
        }
        var history = new History().FromFileChooseResult(jpgResult).WithValue(value);
        await _historyRepository.SaveAsync(history);
        HistoryItems.Insert(0, history);
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task SelectPngAsync(CancellationToken cancellationToken = default)
    {
        using var pngResult = await _fileChoose.GetImageAsync(EFileFormat.Png, cancellationToken);
        if (pngResult == null || !pngResult.HasStream) return;
        using var skBitmap = SKBitmap.Decode(pngResult.Stream);
        string? value = await ScanAndShowQrCode(skBitmap);
        if (string.IsNullOrWhiteSpace(value))
        {
            await _alert.ShowAsync("Aviso", "Não foi encontrado QRCODE no arquivo.");
            return;
        }
        var history = new History().FromFileChooseResult(pngResult).WithValue(value);
        await _historyRepository.SaveAsync(history);
        HistoryItems.Insert(0, history);
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task RecentSelectedItemAsync()
    {
        var selectedItem = SelectedItem;

        SelectedItem = null;

        if (string.IsNullOrWhiteSpace(selectedItem?.Value)) return;

        bool accept = await _alert.AcceptAsync("Copiar para área de transferência?", selectedItem.Value, "Sim", "Não");

        if (accept)
            await _clipboard.SetTextAsync(selectedItem.Value);
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task GoToHistoryTabAsync()
    {
        await _navigation.GoToHistoryTabAsync();
    }

    public async Task LoadRecentsAsync()
    {
        var recents = await _historyRepository.GetRecents(5);// Valor arbitrário para limitar a quantidade de itens recentes

        HistoryItems.Clear();

        foreach (var recent in recents)
            HistoryItems.Add(recent);
    }

    private async Task<string?> ScanAndShowQrCode(SKBitmap skBitmap)
    {
        string? text = await _qrCode.ScanAsync(skBitmap);
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;

        bool accept = await _alert.AcceptAsync("Copiar para área de transferência?",
            text ?? "Não foi encontrado QRCODE no arquivo.", "Sim", "Não");

        if (accept)
            await _clipboard.SetTextAsync(text);

        return text;
    }
}
