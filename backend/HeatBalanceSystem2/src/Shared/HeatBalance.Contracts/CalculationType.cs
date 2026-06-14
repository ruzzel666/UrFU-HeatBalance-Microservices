namespace HeatBalance.Contracts;

public enum CalculationType
{
    ConveyorKiln = 1,
    ChamberKiln = 2,
    ElectricKiln = 3,
    DrumDryer = 4
}

public static class CalculationTypeExtensions
{
    public static string ToRoutePrefix(this CalculationType type) => type switch
    {
        CalculationType.ConveyorKiln => "conveyor",
        CalculationType.ChamberKiln => "chamber",
        CalculationType.ElectricKiln => "electric",
        CalculationType.DrumDryer => "drum",
        _ => throw new ArgumentOutOfRangeException(nameof(type))
    };

    public static string ToDisplayNameRu(this CalculationType type) => type switch
    {
        CalculationType.ConveyorKiln => "Конвейерная сушильная печь",
        CalculationType.ChamberKiln => "Камерная сушильная печь",
        CalculationType.ElectricKiln => "Электрическая сушильная печь",
        CalculationType.DrumDryer => "Сушильный барабан",
        _ => type.ToString()
    };
}
