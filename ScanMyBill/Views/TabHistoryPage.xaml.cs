using ScanMyBill.Core.ViewModels;

namespace ScanMyBill.Views;

public partial class TabHistoryPage : ContentPage
{
    public TabHistoryPage(TabHistoryPageViewModel tabHistoryPageViewModel)
    {
        InitializeComponent();

        BindingContext = tabHistoryPageViewModel;
    }
}
