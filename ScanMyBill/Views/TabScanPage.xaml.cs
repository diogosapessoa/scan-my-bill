using ScanMyBill.Core.ViewModels;

namespace ScanMyBill.Views;

public partial class TabScanPage : ContentPage
{
    public TabScanPage(TabScanPageViewModel tabScanPageViewModel)
    {
        InitializeComponent();

        BindingContext = tabScanPageViewModel;
    }
}
