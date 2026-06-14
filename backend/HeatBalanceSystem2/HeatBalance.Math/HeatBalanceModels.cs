using System.ComponentModel.DataAnnotations;

namespace HeatBalance.Math;

public enum DryerUnitType
{
    ConveyorKiln = 1,
    ChamberKiln = 2,
    ElectricKiln = 3,
    DrumDryer = 4
}

public sealed class ConveyorKilnInput
{
    [Range(0.0001, double.MaxValue)]
    public double WetMaterialMassFlowKgPerHour { get; set; }

    [Range(0.0, 0.9999)]
    public double MoistureInMassFraction { get; set; }

    [Range(0.0, 0.9999)]
    public double MoistureOutMassFraction { get; set; }

    public double MaterialTempInC { get; set; }
    public double MaterialTempOutC { get; set; }

    [Range(0.0, 1.0)]
    public double HeatLossFraction { get; set; } = 0.15;

    [Range(0.0001, double.MaxValue)]
    public double FuelLowerHeatingValueKJPerKg { get; set; } = 42000;

    [Range(0.0, double.MaxValue)]
    public double FuelMassFlowKgPerHour { get; set; }
}

public sealed class ChamberKilnInput
{
    [Range(0.0001, double.MaxValue)]
    public double WetMaterialMassFlowKgPerHour { get; set; }

    [Range(0.0, 0.9999)]
    public double MoistureInMassFraction { get; set; }

    [Range(0.0, 0.9999)]
    public double MoistureOutMassFraction { get; set; }

    public double MaterialTempInC { get; set; }
    public double MaterialTempOutC { get; set; }

    [Range(0.0, 1.0)]
    public double HeatLossFraction { get; set; } = 0.2;

    [Range(0.0001, double.MaxValue)]
    public double FuelLowerHeatingValueKJPerKg { get; set; } = 42000;

    [Range(0.0, double.MaxValue)]
    public double FuelMassFlowKgPerHour { get; set; }
}

public sealed class ElectricKilnInput
{
    [Range(0.0001, double.MaxValue)]
    public double WetMaterialMassFlowKgPerHour { get; set; }

    [Range(0.0, 0.9999)]
    public double MoistureInMassFraction { get; set; }

    [Range(0.0, 0.9999)]
    public double MoistureOutMassFraction { get; set; }

    public double MaterialTempInC { get; set; }
    public double MaterialTempOutC { get; set; }

    [Range(0.0, 1.0)]
    public double HeatLossFraction { get; set; } = 0.1;

    [Range(0.0, double.MaxValue)]
    public double ElectricPowerKw { get; set; }
}

public sealed class DrumDryerInput
{
    [Range(0.0001, double.MaxValue)]
    public double WetMaterialMassFlowKgPerHour { get; set; }

    [Range(0.0, 0.9999)]
    public double MoistureInMassFraction { get; set; }

    [Range(0.0, 0.9999)]
    public double MoistureOutMassFraction { get; set; }

    public double MaterialTempInC { get; set; }
    public double MaterialTempOutC { get; set; }

    [Range(0.0, 1.0)]
    public double HeatLossFraction { get; set; } = 0.18;

    [Range(0.0001, double.MaxValue)]
    public double FuelLowerHeatingValueKJPerKg { get; set; } = 42000;

    [Range(0.0, double.MaxValue)]
    public double FuelMassFlowKgPerHour { get; set; }
}

public sealed record HeatBalanceComponent(string Name, double ValueKJPerHour);

public sealed record HeatBalanceResult(
    DryerUnitType UnitType,
    double TotalInputKJPerHour,
    double UsefulHeatKJPerHour,
    double LossesKJPerHour,
    double UnaccountedKJPerHour,
    double Efficiency,
    IReadOnlyList<HeatBalanceComponent> Components,
    string Notes
);

