using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ScanMyBill.Entities;
using ScanMyBill.Enums;
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

        public TabHistoryPageViewModel(IHistoryRepository historyRepository)
        {
            _historyRepository = historyRepository;
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
    }
}
