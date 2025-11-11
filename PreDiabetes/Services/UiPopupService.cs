using CommunityToolkit.Maui.Views;

namespace PreDiabetes.Services;

public class UiPopupService : IUiPopupService
{
    readonly IServiceProvider _sp;
    public UiPopupService(IServiceProvider sp) => _sp = sp;

    public void ShowResult(int points, string? riskFactor, string? message)
    {
        // create popup via DI so it can get services if needed
        var popup = ActivatorUtilities.CreateInstance<PreDiabetes.Pages.PreDiabetes.ResultPopup>(_sp, points, riskFactor ?? string.Empty, message ?? string.Empty);

        var host = Shell.Current as Page;
        if (host == null && Application.Current?.Windows?.Count > 0)
            host = Application.Current.Windows[0].Page;

        host?.ShowPopup(popup);
    }
}
