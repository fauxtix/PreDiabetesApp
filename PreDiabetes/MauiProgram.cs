using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using PreDiabetes.Pages.Languages;
using PreDiabetes.Pages.PreDiabetes;
using PreDiabetes.ViewModels;
using PreDiabetes.ViewModels.Languages;
using Syncfusion.Maui.Toolkit.Hosting;
using System.Globalization;

namespace PreDiabetes
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {

            CultureInfo cultureInfo = new CultureInfo("pt-PT");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureSyncfusionToolkit()
                .ConfigureMauiHandlers(handlers =>
                {
#if IOS || MACCATALYST
                    handlers.AddHandler<Microsoft.Maui.Controls.CollectionView, Microsoft.Maui.Controls.Handlers.Items2.CollectionViewHandler2>();
#endif
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
                    fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
                });

#if DEBUG
            builder.Logging.AddDebug();
            builder.Services.AddLogging(configure => configure.AddDebug());
#endif

            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources/Languages");
            var savedCulture = Preferences.Get("AppLanguage", null);
            if (string.IsNullOrEmpty(savedCulture))
            {
                savedCulture = System.Globalization.CultureInfo.CurrentCulture.Name;
                Preferences.Set("AppLanguage", savedCulture);
            }
            var culture = new System.Globalization.CultureInfo(savedCulture);
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = culture;
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = culture;


            builder.Services.AddSingleton<LanguageSettingsViewModel>();


            builder.Services.AddSingleton<IPreDiabetesCalculatorService, PreDiabetesCalculatorService>();
            builder.Services.AddSingleton<INavigationService, NavigationService>();

            builder.Services.AddTransient<PreDiabetesViewModel>();
            builder.Services.AddTransient<PreDiabetesIntroViewModel>();
            builder.Services.AddTransient<PreDiabetesPage>();
            builder.Services.AddTransient<PreDiabetesIntroPage>();
            builder.Services.AddTransient<LanguageSettingsPage>();


            builder.Services.AddTransient<ResultPopup>();
            builder.Services.AddTransient<ResultPopupViewModel>();
            return builder.Build();
        }
    }
}
