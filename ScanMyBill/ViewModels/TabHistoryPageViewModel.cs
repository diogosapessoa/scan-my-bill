using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ScanMyBill.Entities;
using ScanMyBill.Enums;
using ScanMyBill.Interfaces;
using ScanMyBill.Repositories;

using System.Collections.ObjectModel;

namespace ScanMyBill.ViewModels
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

        public ObservableCollection<History> HistoryItems { get; set; } = new();

        private readonly IHistoryRepository _historyRepository;
        private readonly IAlert _alert;
        private readonly IClipboard _clipboard;

        public TabHistoryPageViewModel(IHistoryRepository historyRepository, IAlert alert, IClipboard clipboard)
        {
            _historyRepository = historyRepository;
            _alert = alert;
            _clipboard = clipboard;
        }

        public async Task LodHistoryWithFilters()
        {
            var recents = await _historyRepository.GetAllByFilters(FilterFormat, InputSearch);

            HistoryItems.Clear();

            foreach (var recent in recents)
                HistoryItems.Add(recent);
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task SearchEnterAsync()
        {
            await LodHistoryWithFilters();
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task FilterAllSelectedAsync()
        {
            FilterFormat = EFileFormat.Undefined;
            IsFilterAll = true;
            IsFilterImage = false;
            IsFilterPdf = false;

            await LodHistoryWithFilters();
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task FilterPdfSelectedAsync()
        {
            FilterFormat = EFileFormat.Pdf;
            IsFilterPdf = true;
            IsFilterAll = false;
            IsFilterImage = false;

            await LodHistoryWithFilters();
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task FilterImageSelectedAsync()
        {
            FilterFormat = EFileFormat.Png;
            IsFilterImage = true;
            IsFilterAll = false;
            IsFilterPdf = false;

            await LodHistoryWithFilters();
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task HistorySelectedItemAsync(History history)
        {
            if (history == null) return;

            //Em teoria não deve acontecer, só deveria ter salvo com campo valor
            if (string.IsNullOrWhiteSpace(history.Value))
            {
                if (await _alert.AcceptAsync("Deseja deletar?", history.Name, "Sim", "Não"))
                {
                    await _historyRepository.DeleteByIdAsync(history.Id);
                    HistoryItems.Remove(history);
                }
            }
            else
            {
                string choice = await _alert.ShowActionAsync(history.Name, "Cancelar", "Deletar", "Copiar para área de transferência");

                if (string.IsNullOrWhiteSpace(choice)) return;

                if (choice.StartsWith("Deletar"))
                {
                    await _historyRepository.DeleteByIdAsync(history.Id);
                    HistoryItems.Remove(history);
                }

                if (choice.StartsWith("Copiar") && !string.IsNullOrWhiteSpace(history.Value))
                    await _clipboard.SetTextAsync(history.Value);
            }
        }
    }
}
