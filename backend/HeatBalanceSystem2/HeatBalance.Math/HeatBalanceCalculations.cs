namespace HeatBalance.Math;

public static class HeatBalanceCalculations
{
    public static HeatBalanceResult Calculate(ConveyorKilnInput input)
        => CalculateCommon(DryerUnitType.ConveyorKiln,
            wetMaterialMassFlowKgPerHour: input.WetMaterialMassFlowKgPerHour,
            moistureInMassFraction: input.MoistureInMassFraction,
            moistureOutMassFraction: input.MoistureOutMassFraction,
            materialTempInC: input.MaterialTempInC,
            materialTempOutC: input.MaterialTempOutC,
            totalInputKJPerHour: input.FuelMassFlowKgPerHour * input.FuelLowerHeatingValueKJPerKg,
            heatLossFraction: input.HeatLossFraction);

    public static HeatBalanceResult Calculate(ChamberKilnInput input)
        => CalculateCommon(DryerUnitType.ChamberKiln,
            wetMaterialMassFlowKgPerHour: input.WetMaterialMassFlowKgPerHour,
            moistureInMassFraction: input.MoistureInMassFraction,
            moistureOutMassFraction: input.MoistureOutMassFraction,
            materialTempInC: input.MaterialTempInC,
            materialTempOutC: input.MaterialTempOutC,
            totalInputKJPerHour: input.FuelMassFlowKgPerHour * input.FuelLowerHeatingValueKJPerKg,
            heatLossFraction: input.HeatLossFraction);

    public static HeatBalanceResult Calculate(ElectricKilnInput input)
        => CalculateCommon(DryerUnitType.ElectricKiln,
            wetMaterialMassFlowKgPerHour: input.WetMaterialMassFlowKgPerHour,
            moistureInMassFraction: input.MoistureInMassFraction,
            moistureOutMassFraction: input.MoistureOutMassFraction,
            materialTempInC: input.MaterialTempInC,
            materialTempOutC: input.MaterialTempOutC,
            totalInputKJPerHour: input.ElectricPowerKw * 3600.0,
            heatLossFraction: input.HeatLossFraction);

    public static HeatBalanceResult Calculate(DrumDryerInput input)
        => CalculateCommon(DryerUnitType.DrumDryer,
            wetMaterialMassFlowKgPerHour: input.WetMaterialMassFlowKgPerHour,
            moistureInMassFraction: input.MoistureInMassFraction,
            moistureOutMassFraction: input.MoistureOutMassFraction,
            materialTempInC: input.MaterialTempInC,
            materialTempOutC: input.MaterialTempOutC,
            totalInputKJPerHour: input.FuelMassFlowKgPerHour * input.FuelLowerHeatingValueKJPerKg,
            heatLossFraction: input.HeatLossFraction);

    private static HeatBalanceResult CalculateCommon(
        DryerUnitType unitType,
        double wetMaterialMassFlowKgPerHour,
        double moistureInMassFraction,
        double moistureOutMassFraction,
        double materialTempInC,
        double materialTempOutC,
        double totalInputKJPerHour,
        double heatLossFraction)
    {
        if (wetMaterialMassFlowKgPerHour <= 0) throw new ArgumentOutOfRangeException(nameof(wetMaterialMassFlowKgPerHour));
        if (moistureInMassFraction < 0 || moistureInMassFraction >= 1) throw new ArgumentOutOfRangeException(nameof(moistureInMassFraction));
        if (moistureOutMassFraction < 0 || moistureOutMassFraction >= 1) throw new ArgumentOutOfRangeException(nameof(moistureOutMassFraction));
        if (moistureOutMassFraction > moistureInMassFraction) throw new ArgumentException("Конечная влажность не может быть больше начальной.");
        if (totalInputKJPerHour < 0) throw new ArgumentOutOfRangeException(nameof(totalInputKJPerHour));
        if (heatLossFraction < 0 || heatLossFraction > 1) throw new ArgumentOutOfRangeException(nameof(heatLossFraction));

        // Конвенции: влажность задаётся на "влажной основе" (масса воды / масса мокрого материала).
        const double CpDryKJPerKgK = 1.3;     // условная теплоёмкость сухого вещества
        const double CpWaterKJPerKgK = 4.186; // теплоёмкость воды
        const double LatentKJPerKg = 2257.0;  // теплота парообразования при ~100°C

        var mDry = wetMaterialMassFlowKgPerHour * (1.0 - moistureInMassFraction);
        var mWaterIn = wetMaterialMassFlowKgPerHour * moistureInMassFraction;
        var mWaterOut = mDry * moistureOutMassFraction / (1.0 - moistureOutMassFraction);
        var mEvap = System.Math.Max(0.0, mWaterIn - mWaterOut);

        var dTMaterial = materialTempOutC - materialTempInC;

        var qHeatDry = mDry * CpDryKJPerKgK * dTMaterial;
        var qHeatEvapWaterTo100 = mEvap * CpWaterKJPerKgK * (100.0 - materialTempInC);
        var qLatent = mEvap * LatentKJPerKg;

        var useful = qHeatDry + qHeatEvapWaterTo100 + qLatent;
        var losses = totalInputKJPerHour * heatLossFraction;
        var unaccounted = totalInputKJPerHour - useful - losses;

        var notes = string.Empty;
        if (unaccounted < 0)
        {
            notes =
                "ВНИМАНИЕ: по введённым данным требуемая полезная теплота превышает подводимую энергию. " +
                "Проверьте расход топлива/мощность и исходные параметры.";
        }

        var efficiency = totalInputKJPerHour <= 0 ? 0 : System.Math.Clamp(useful / totalInputKJPerHour, 0, 1);

        var components = new List<HeatBalanceComponent>
        {
            new("Нагрев сухого материала", qHeatDry),
            new("Нагрев испаряющейся влаги до 100°C", qHeatEvapWaterTo100),
            new("Парообразование влаги", qLatent),
            new("Потери (условно)", losses),
            new("Невязка/прочее", unaccounted)
        };

        return new HeatBalanceResult(
            UnitType: unitType,
            TotalInputKJPerHour: totalInputKJPerHour,
            UsefulHeatKJPerHour: useful,
            LossesKJPerHour: losses,
            UnaccountedKJPerHour: unaccounted,
            Efficiency: efficiency,
            Components: components,
            Notes: notes
        );
    }
}

