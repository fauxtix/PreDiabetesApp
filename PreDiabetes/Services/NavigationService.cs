namespace PreDiabetes.Services;

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _services;

    public NavigationService(IServiceProvider services)
    {
        _services = services;
    }

    public async Task NavigateToAsync<TPage>() where TPage : Page
    {
        var page = (Page)_services.GetRequiredService(typeof(TPage));
        await Shell.Current.Navigation.PushAsync(page);
    }

    public async Task NavigateToAsync(Type pageType)
    {
        var page = (Page)_services.GetRequiredService(pageType);
        await Shell.Current.Navigation.PushAsync(page);
    }
}
