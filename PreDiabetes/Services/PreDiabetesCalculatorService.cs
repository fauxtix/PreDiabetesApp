using PreDiabetes.Models;
using System.Globalization;
using System.Text;

namespace PreDiabetes.Services;

public class PreDiabetesCalculatorService : IPreDiabetesCalculatorService
{
    public PreDiabetesResult Calculate(PreDiabetesInput input)
    {
        int pontosIdade = MapearPontosIdade(input.AgeIndex);

        int pontosImc = (input.WeightKg > 0 && input.HeightCm > 0)
            ? PontosImcPorPesoAltura(input.WeightKg, input.HeightCm)
            : EstimarPontosImcAPartirIndiceCintura(input.GenderIndex, input.ManWaistIndex, input.WomanWaistIndex);

        int pontosCintura = MapearPontosCintura(input.GenderIndex, input.ManWaistIndex, input.WomanWaistIndex);
        int pontosAtividade = input.AtividadeFisica ? 0 : 2;
        int pontosVegetais = input.VegetaisTodosOsDias ? 0 : 1;
        int pontosHipertensao = input.HipertensaoMedication ? 2 : 0;
        int pontosGlucose = input.GlucoseInTests ? 5 : 0;
        int pontosParentesco = input.ParenteDiabetes ? 5 : 0; // conservador: boolean -> 5

        int total = pontosIdade + pontosImc + pontosCintura + pontosAtividade
                    + pontosVegetais + pontosHipertensao + pontosGlucose + pontosParentesco;

        var (msg, rotulo) = ObterMensagemFindrisc(total);
        return new PreDiabetesResult(total, rotulo, msg);
    }

    private static int MapearPontosIdade(int ageIndex)
    {
        return ageIndex switch
        {
            0 => 0,
            1 => 0,
            2 => 2,
            3 => 3,
            _ => 4
        };
    }
    private static int PontosImcPorPesoAltura(double weightKg, double heightCm)
    {
        if (weightKg <= 0 || heightCm <= 0) return 0;
        var h = heightCm / 100.0;
        var bmi = weightKg / (h * h);
        if (bmi < 25.0) return 0;
        if (bmi < 30.0) return 1;
        return 3;
    }

    private static int EstimarPontosImcAPartirIndiceCintura(int genderIndex, int manWaistIndex, int womanWaistIndex)
    {
        int idx = genderIndex == 1 ? manWaistIndex : womanWaistIndex;
        return idx switch { 0 => 0, 1 => 1, _ => 3 };
    }

    private static int MapearPontosCintura(int genderIndex, int manWaistIndex, int womanWaistIndex)
    {
        int idx = genderIndex == 1 ? manWaistIndex : womanWaistIndex;
        return idx switch { 0 => 0, 1 => 3, _ => 4 };
    }

    private static (string Message, string Rotulo) ObterMensagemFindrisc(int total)
    {
        var twoLetter = (CultureInfo.CurrentUICulture ?? CultureInfo.CurrentCulture).TwoLetterISOLanguageName;
        var isPt = twoLetter.Equals("pt", StringComparison.OrdinalIgnoreCase);

        var sb = new StringBuilder();

        if (isPt)
        {
            if (total <= 6)
                return ("Risco baixo a 10 anos (estimativa FINDRISC): aproximadamente <5% de desenvolver diabetes.", "Baixo");

            if (total <= 11)
            {
                sb.AppendLine("Risco ligeiramente elevado a 10 anos (FINDRISC).");
                sb.AppendLine("Estimativa aproximada: ~4% (varia por população).");
                return (sb.ToString(), "Ligeiramente elevado");
            }

            if (total <= 14)
            {
                sb.AppendLine("Risco moderado a 10 anos (FINDRISC).");
                sb.AppendLine("Estimativa aproximada: ~17%.");
                return (sb.ToString(), "Moderado");
            }

            if (total <= 20)
            {
                sb.AppendLine("Risco alto a 10 anos (FINDRISC).");
                sb.AppendLine("Estimativa aproximada: ~33%.");
                return (sb.ToString(), "Alto");
            }

            sb.AppendLine("Risco muito alto a 10 anos (FINDRISC).");
            sb.AppendLine("Estimativa aproximada: >50%.");
            return (sb.ToString(), "Muito alto");
        }
        else
        {
            if (total <= 6)
                return ("Low risk over 10 years (FINDRISC estimate): about <5% chance of developing diabetes.", "Low");

            if (total <= 11)
            {
                sb.AppendLine("Slightly elevated risk over 10 years (FINDRISC).");
                sb.AppendLine("Estimated probability: ~4% (varies by population).");
                return (sb.ToString(), "Slightly elevated");
            }

            if (total <= 14)
            {
                sb.AppendLine("Moderate risk over 10 years (FINDRISC).");
                sb.AppendLine("Estimated probability: ~17%.");
                return (sb.ToString(), "Moderate");
            }

            if (total <= 20)
            {
                sb.AppendLine("High risk over 10 years (FINDRISC).");
                sb.AppendLine("Estimated probability: ~33%.");
                return (sb.ToString(), "High");
            }

            sb.AppendLine("Very high risk over 10 years (FINDRISC).");
            sb.AppendLine("Estimated probability: >50%.");
            return (sb.ToString(), "Very high");
        }
    }
}
