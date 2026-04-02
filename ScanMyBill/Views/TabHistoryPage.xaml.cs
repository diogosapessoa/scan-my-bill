using ScanMyBill.ViewModels;

namespace ScanMyBill.Views;

public partial class TabHistoryPage : ContentPage
{
    private readonly TabHistoryPageViewModel _viewModel;

    public TabHistoryPage(TabHistoryPageViewModel tabHistoryPageViewModel)
    {
        InitializeComponent();

        _viewModel = tabHistoryPageViewModel;

        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.LodHistoryWithFilters();
    }
}