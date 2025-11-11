namespace PreDiabetes.Services;

public interface IUiPopupService
{
    void ShowResult(int points, string? riskFactor, string? message);
}
