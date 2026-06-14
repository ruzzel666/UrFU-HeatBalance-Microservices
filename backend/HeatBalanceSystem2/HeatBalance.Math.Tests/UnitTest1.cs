using HeatBalance.Math;

namespace HeatBalance.Math.Tests;

public class UnitTest1
{
    [Fact]
    public void ConveyorKiln_SampleCase_IsStable()
    {
        var input = new ConveyorKilnInput
        {
            WetMaterialMassFlowKgPerHour = 1000,
            MoistureInMassFraction = 0.30,
            MoistureOutMassFraction = 0.10,
            MaterialTempInC = 20,
            MaterialTempOutC = 80,
            FuelLowerHeatingValueKJPerKg = 42000,
            FuelMassFlowKgPerHour = 20,
            HeatLossFraction = 0.15
        };

        var result = HeatBalanceCalculations.Calculate(input);

        Assert.Equal(DryerUnitType.ConveyorKiln, result.UnitType);
        Assert.InRange(result.TotalInputKJPerHour, 839_999.9, 840_000.1);

        // Полезная теплота (расчёт по формуле из библиотеки)
        Assert.InRange(result.UsefulHeatKJPerHour, 630_000, 632_000);

        // Потери как доля подвода
        Assert.InRange(result.LossesKJPerHour, 125_999.9, 126_000.1);

        // Невязка должна быть положительной для данного набора
        Assert.True(result.UnaccountedKJPerHour > 0);
    }

    [Fact]
    public void ChamberKiln_SampleCase_IsStable()
    {
        var input = new ChamberKilnInput
        {
            WetMaterialMassFlowKgPerHour = 1000,
            MoistureInMassFraction = 0.30,
            MoistureOutMassFraction = 0.10,
            MaterialTempInC = 20,
            MaterialTempOutC = 80,
            FuelLowerHeatingValueKJPerKg = 42000,
            FuelMassFlowKgPerHour = 20,
            HeatLossFraction = 0.20
        };

        var result = HeatBalanceCalculations.Calculate(input);

        Assert.Equal(DryerUnitType.ChamberKiln, result.UnitType);
        Assert.InRange(result.TotalInputKJPerHour, 839_999.9, 840_000.1);
        Assert.InRange(result.LossesKJPerHour, 167_999.9, 168_000.1);
        Assert.True(result.UnaccountedKJPerHour > 0);
    }

    [Fact]
    public void ElectricKiln_SampleCase_IsStable()
    {
        var input = new ElectricKilnInput
        {
            WetMaterialMassFlowKgPerHour = 1000,
            MoistureInMassFraction = 0.30,
            MoistureOutMassFraction = 0.10,
            MaterialTempInC = 20,
            MaterialTempOutC = 80,
            ElectricPowerKw = 233.3333333333, // 233.333.. * 3600 = 840000 кДж/ч
            HeatLossFraction = 0.10
        };

        var result = HeatBalanceCalculations.Calculate(input);

        Assert.Equal(DryerUnitType.ElectricKiln, result.UnitType);
        Assert.InRange(result.TotalInputKJPerHour, 839_999.0, 840_001.0);
        Assert.InRange(result.LossesKJPerHour, 83_999.0, 84_001.0);
        Assert.True(result.UnaccountedKJPerHour > 0);
    }

    [Fact]
    public void DrumDryer_SampleCase_IsStable()
    {
        var input = new DrumDryerInput
        {
            WetMaterialMassFlowKgPerHour = 1000,
            MoistureInMassFraction = 0.30,
            MoistureOutMassFraction = 0.10,
            MaterialTempInC = 20,
            MaterialTempOutC = 80,
            FuelLowerHeatingValueKJPerKg = 42000,
            FuelMassFlowKgPerHour = 20,
            HeatLossFraction = 0.18
        };

        var result = HeatBalanceCalculations.Calculate(input);

        Assert.Equal(DryerUnitType.DrumDryer, result.UnitType);
        Assert.InRange(result.TotalInputKJPerHour, 839_999.9, 840_000.1);
        Assert.InRange(result.LossesKJPerHour, 151_199.9, 151_200.1);
        Assert.True(result.UnaccountedKJPerHour > 0);
    }
}
