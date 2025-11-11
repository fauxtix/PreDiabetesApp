using CommunityToolkit.Maui.Views;
using PreDiabetes.ViewModels;

namespace PreDiabetes.Pages.PreDiabetes;

public partial class ResultPopup : Popup
{
    ResultPopupViewModel? _vm;

    public ResultPopup(int points, string riskFactor, string message, string? bmiText = null, string? bmiCategory = null)
    {
        InitializeComponent();

        var display = DeviceDisplay.MainDisplayInfo;
        var density = display.Density;
        var width = display.Width / density;
        var height = display.Height / density;
        Size = new Size(width, height);

        _vm = new ResultPopupViewModel(points, riskFactor, message, bmiText, bmiCategory);
        BindingContext = _vm;

        _vm.RequestClose += async () => await ClosePopupAsync();

        Dispatcher.Dispatch(async () =>
        {
            try
            {
                await ContentFrame.FadeTo(1, 180, Easing.CubicOut);
                await ContentFrame.ScaleTo(1.00, 220, Easing.SpringOut);
            }
            catch { }
        });
    }

    async Task ClosePopupAsync()
    {
        try
        {
            await ContentFrame.ScaleTo(0.94, 120, Easing.CubicIn);
            await ContentFrame.FadeTo(0, 120, Easing.CubicIn);
        }
        catch { }
        Close();
    }

    void OnOverlayTapped(object? sender, EventArgs e)
    {
        _vm?.CloseCommand.Execute(null);
    }
}