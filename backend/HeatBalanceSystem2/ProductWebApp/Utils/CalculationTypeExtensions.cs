using ProductWebApp.Data;

namespace ProductWebApp.Utils;

public static class CalculationTypeExtensions
{
    public static string ToDisplayName(this CalculationType type) => type switch
    {
        CalculationType.ConveyorKiln => "Конвейерная сушильная печь",
        CalculationType.ChamberKiln => "Камерная сушильная печь",
        CalculationType.ElectricKiln => "Электрическая сушильная печь",
        CalculationType.DrumDryer => "Сушильный барабан",
        _ => type.ToString()
    };
}

