using ScanMyBill.Views;

namespace ScanMyBill;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(TabScanPage), typeof(TabScanPage));
        Routing.RegisterRoute(nameof(TabHistoryPage), typeof(TabHistoryPage));
    }
}
