using ScanMyBill.Data;
using ScanMyBill.Entities;

namespace ScanMyBill;

public partial class App : Application
{
    public App()
    {
        //Manter o estilo claro, mesmo que o sistema esteja no modo escuro
        Current?.UserAppTheme = AppTheme.Light;

        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new AppShell());

#if WINDOWS // Definir o tamanho da janela para 9:16 ao iniciar no windwos
        window.Width = 360;
        window.Height = 640;
#endif

        return window;
    }

    protected override async void OnStart()
    {
        await DatabaseHelper.Connection.CreateTableAsync<History>();

        base.OnStart();
    }
}
