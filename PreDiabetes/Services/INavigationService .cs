namespace PreDiabetes.Services;

public interface INavigationService
{
    Task NavigateToAsync<TPage>() where TPage : Page;
    Task NavigateToAsync(Type pageType);
}
