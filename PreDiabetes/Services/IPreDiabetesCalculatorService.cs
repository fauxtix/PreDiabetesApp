using PreDiabetes.Models;

namespace PreDiabetes.Services;

public interface IPreDiabetesCalculatorService
{
    PreDiabetesResult Calculate(PreDiabetesInput input);
}
