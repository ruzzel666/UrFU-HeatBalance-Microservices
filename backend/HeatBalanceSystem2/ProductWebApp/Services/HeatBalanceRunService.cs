using System.Text.Json;
using HeatBalance.Math;
using ProductWebApp.Data;

namespace ProductWebApp.Services;

public sealed class HeatBalanceRunService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    public string SerializeInput(CalculationType type, object input)
    {
        _ = type;
        return JsonSerializer.Serialize(input, JsonOptions);
    }

    public object DeserializeInput(InputDataSet dataSet)
    {
        return dataSet.CalculationType switch
        {
            CalculationType.ConveyorKiln => JsonSerializer.Deserialize<ConveyorKilnInput>(dataSet.InputJson, JsonOptions)
                                            ?? throw new InvalidOperationException("Не удалось прочитать входные данные (ConveyorKilnInput)."),
            CalculationType.ChamberKiln => JsonSerializer.Deserialize<ChamberKilnInput>(dataSet.InputJson, JsonOptions)
                                           ?? throw new InvalidOperationException("Не удалось прочитать входные данные (ChamberKilnInput)."),
            CalculationType.ElectricKiln => JsonSerializer.Deserialize<ElectricKilnInput>(dataSet.InputJson, JsonOptions)
                                            ?? throw new InvalidOperationException("Не удалось прочитать входные данные (ElectricKilnInput)."),
            CalculationType.DrumDryer => JsonSerializer.Deserialize<DrumDryerInput>(dataSet.InputJson, JsonOptions)
                                         ?? throw new InvalidOperationException("Не удалось прочитать входные данные (DrumDryerInput)."),
            _ => throw new NotSupportedException($"Неизвестный тип расчёта: {dataSet.CalculationType}")
        };
    }

    public HeatBalanceResult Calculate(InputDataSet dataSet)
    {
        return DeserializeInput(dataSet) switch
        {
            ConveyorKilnInput i => HeatBalanceCalculations.Calculate(i),
            ChamberKilnInput i => HeatBalanceCalculations.Calculate(i),
            ElectricKilnInput i => HeatBalanceCalculations.Calculate(i),
            DrumDryerInput i => HeatBalanceCalculations.Calculate(i),
            _ => throw new InvalidOperationException("Не удалось подобрать калькулятор под входные данные.")
        };
    }

    public string SerializeResult(HeatBalanceResult result)
        => JsonSerializer.Serialize(result, JsonOptions);

    public HeatBalanceResult DeserializeResult(string json)
        => JsonSerializer.Deserialize<HeatBalanceResult>(json, JsonOptions)
           ?? throw new InvalidOperationException("Не удалось прочитать результат расчёта.");
}

