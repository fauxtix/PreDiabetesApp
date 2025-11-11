namespace PreDiabetes.Models
{
    public record PreDiabetesInput
    {
        public int AgeIndex { get; init; }
        public int GenderIndex { get; init; } // 0 = Female, 1 = Male
        public int ManWaistIndex { get; init; } // 0,1,2
        public int WomanWaistIndex { get; init; } // 0,1,2
        public bool IsSmoker { get; init; }
        public bool HipertensaoMedication { get; init; }
        public bool GlucoseInTests { get; init; }
        public bool ParenteDiabetes { get; init; }
        public bool AtividadeFisica { get; init; } // true = performs 30 min/day
        public bool VegetaisTodosOsDias { get; init; } // true = every day

        // NEW: peso (kg) e altura (cm) — opcionais
        public double WeightKg { get; init; }
        public double HeightCm { get; init; }
    }

    public record PreDiabetesResult(int Points, string RiskFactor, string Message);
}