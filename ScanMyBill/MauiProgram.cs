using CommunityToolkit.Maui;

using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;

using ScanMyBill.Interfaces;
using ScanMyBill.Repositories;
using ScanMyBill.Services;
using ScanMyBill.ViewModels;

namespace ScanMyBill;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Inter.ttf", "Inter");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<IAlert, AlertService>();
        builder.Services.AddSingleton<IFileChoose, FileChooseService>();
        builder.Services.AddSingleton<IPdf, PdfToImageService>();
        builder.Services.AddSingleton<IQrCode, QrCodeService>();

        builder.Services.AddSingleton(Clipboard.Default);
        builder.Services.AddSingleton<IHistoryRepository, HistoryRepository>();

        builder.Services.AddTransient<TabScanPageViewModel>();
        builder.Services.AddTransient<TabHistoryPageViewModel>();

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

        return builder.Build();
    }
}
