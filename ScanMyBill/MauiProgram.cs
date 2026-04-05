using CommunityToolkit.Maui;

using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;

using Plugin.AdMob;
using Plugin.AdMob.Configuration;

using ScanMyBill.Core.Interfaces;
using ScanMyBill.Core.Repositories;
using ScanMyBill.Core.ViewModels;
using ScanMyBill.Repositories;
using ScanMyBill.Services;

namespace ScanMyBill;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseAdMob()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Inter.ttf", "Inter");
            });

#if DEBUG
        builder.Logging.AddDebug();

        AdConfig.UseTestAdUnitIds = true;
        AdConfig.AddTestDevice("18F3F06D35661D6D4556C8F76699738C");
#endif

        DependencyInjection(builder.Services);

        MapHandler();

        return builder.Build();
    }

    private static void DependencyInjection(IServiceCollection services)
    {
        services.AddSingleton<IClipboardService, ClipboardService>();

        services.AddSingleton<IAlert, AlertService>();
        services.AddSingleton<IFileChoose, FileChooseService>();
        services.AddSingleton<IPdf, PdfToImageService>();
        services.AddSingleton<IQrCode, QrCodeService>();
        services.AddSingleton<IAppNavigation, AppNavigationService>();

        services.AddSingleton<IHistoryRepository, HistoryRepository>();

        services.AddTransient<TabScanPageViewModel>();
        services.AddTransient<TabHistoryPageViewModel>();
    }

    private static void MapHandler()
    {
        //Ajuste visual para Entry
        EntryHandler.Mapper.AppendToMapping("NoBorderEntry", (handler, view) =>
        {

#if ANDROID
            handler.PlatformView.Background = null;
            handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
            handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#elif IOS || MACCATALYST
        handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
        handler.PlatformView.Layer.BorderWidth = 0;
        handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif

        });
    }
}
