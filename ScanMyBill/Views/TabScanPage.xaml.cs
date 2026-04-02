using ScanMyBill.ViewModels;

namespace ScanMyBill.Views;

public partial class TabScanPage : ContentPage
{
    private readonly TabScanPageViewModel _viewModel;

    public TabScanPage(TabScanPageViewModel tabScanPageViewModel)
    {
        InitializeComponent();

        _viewModel = tabScanPageViewModel;

        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.LoadRecentsAsync();
    }
}
