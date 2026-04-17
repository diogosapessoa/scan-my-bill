using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ScanMyBill.Core.Entities;
using ScanMyBill.Core.Enums;
using ScanMyBill.Core.Interfaces;
using ScanMyBill.Core.Repositories;

using System.Collections.ObjectModel;

namespace ScanMyBill.Core.ViewModels
{
    public partial class TabHistoryPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? inputSearch;

        [ObservableProperty]
        private EFileFormat filterFormat = EFileFormat.Undefined;

        [ObservableProperty]
        private bool isFilterAll = true;


        [ObservableProperty]
        private bool isFilterPdf;


        [ObservableProperty]
        private bool isFilterImage;

        [ObservableProperty]
        private History? selectedItem;

        public ObservableCollection<History> HistoryItems { get; set; } = new();

        private readonly IHistoryRepository _historyRepository;
        private readonly IAlert _alert;
        private readonly IClipboardService _clipboard;

        public TabHistoryPageViewModel(IHistoryRepository historyRepository, IAlert alert, IClipboardService clipboard)
        {
            _historyRepository = historyRepository;
            _alert = alert;
            _clipboard = clipboard;
        }

        [RelayCommand]
        public async Task OnApeearingAsync()
        {
            await LodHistoryWithFilters();
        }

        public async Task LodHistoryWithFilters()
        {
            var recents = await _historyRepository.GetAllByFilters(FilterFormat, InputSearch);

            HistoryItems.Clear();

            foreach (var recent in recents)
                HistoryItems.Add(recent);
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task SearchEnterAsync()
        {
            await LodHistoryWithFilters();
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task FilterAllSelectedAsync()
        {
            FilterFormat = EFileFormat.Undefined;
            IsFilterAll = true;
            IsFilterImage = false;
            IsFilterPdf = false;

            await LodHistoryWithFilters();
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task FilterPdfSelectedAsync()
        {
            FilterFormat = EFileFormat.Pdf;
            IsFilterPdf = true;
            IsFilterAll = false;
            IsFilterImage = false;

            await LodHistoryWithFilters();
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task FilterImageSelectedAsync()
        {
            FilterFormat = EFileFormat.Png;
            IsFilterImage = true;
            IsFilterAll = false;
            IsFilterPdf = false;

            await LodHistoryWithFilters();
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task HistorySelectedItemAsync()
        {
            if (SelectedItem == null) return;

            var selectedItem = SelectedItem;

            SelectedItem = null;

            if (string.IsNullOrWhiteSpace(selectedItem.Value))
            {
                if (await _alert.AcceptAsync("Deseja deletar?", selectedItem.Name, "Sim", "Não"))
                {
                    await _historyRepository.DeleteByIdAsync(selectedItem.Id);
                    HistoryItems.Remove(selectedItem);
                }
            }
            else
            {
                string choice = await _alert.ShowActionAsync(selectedItem.Name, "Cancelar", "Deletar", "Copiar para área de transferência");

                if (string.IsNullOrWhiteSpace(choice))
                {
                    selectedItem = null;
                    return;
                }

                if (choice.StartsWith("Deletar"))
                {
                    await _historyRepository.DeleteByIdAsync(selectedItem.Id);
                    HistoryItems.Remove(selectedItem);
                }

                if (choice.StartsWith("Copiar") && !string.IsNullOrWhiteSpace(selectedItem.Value))
                    await _clipboard.SetTextAsync(selectedItem.Value);
            }
        }
    }
}
