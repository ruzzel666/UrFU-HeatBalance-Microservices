using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ProductWebApp.ModelBinding;

public sealed class FlexibleNumberModelBinder : IModelBinder
{
    private readonly Type _targetType;

    public FlexibleNumberModelBinder(Type targetType)
    {
        _targetType = targetType;
    }

    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (value == ValueProviderResult.None)
            return Task.CompletedTask;

        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, value);

        var raw = value.FirstValue;
        if (string.IsNullOrWhiteSpace(raw))
            return Task.CompletedTask;

        raw = raw.Trim().Replace(" ", "");

        bool ok;
        object? parsed = null;

        if (_targetType == typeof(double) || _targetType == typeof(double?))
        {
            ok = TryParseDouble(raw, out var d);
            parsed = d;
        }
        else if (_targetType == typeof(float) || _targetType == typeof(float?))
        {
            ok = TryParseFloat(raw, out var f);
            parsed = f;
        }
        else if (_targetType == typeof(decimal) || _targetType == typeof(decimal?))
        {
            ok = TryParseDecimal(raw, out var m);
            parsed = m;
        }
        else
        {
            ok = false;
        }

        if (!ok)
        {
            bindingContext.ModelState.TryAddModelError(bindingContext.ModelName,
                $"Значение '{raw}' некорректно. Используйте десятичную точку, например 0.25.");
            return Task.CompletedTask;
        }

        bindingContext.Result = ModelBindingResult.Success(parsed);
        return Task.CompletedTask;
    }

    private static bool TryParseDouble(string raw, out double value)
    {
        const NumberStyles style = NumberStyles.Float | NumberStyles.AllowThousands;
        return double.TryParse(raw, style, CultureInfo.CurrentCulture, out value)
               || double.TryParse(raw, style, CultureInfo.InvariantCulture, out value)
               || double.TryParse(raw.Replace(',', '.'), style, CultureInfo.InvariantCulture, out value)
               || double.TryParse(raw.Replace('.', ','), style, CultureInfo.GetCultureInfo("ru-RU"), out value);
    }

    private static bool TryParseFloat(string raw, out float value)
    {
        const NumberStyles style = NumberStyles.Float | NumberStyles.AllowThousands;
        return float.TryParse(raw, style, CultureInfo.CurrentCulture, out value)
               || float.TryParse(raw, style, CultureInfo.InvariantCulture, out value)
               || float.TryParse(raw.Replace(',', '.'), style, CultureInfo.InvariantCulture, out value)
               || float.TryParse(raw.Replace('.', ','), style, CultureInfo.GetCultureInfo("ru-RU"), out value);
    }

    private static bool TryParseDecimal(string raw, out decimal value)
    {
        const NumberStyles style = NumberStyles.Number;
        return decimal.TryParse(raw, style, CultureInfo.CurrentCulture, out value)
               || decimal.TryParse(raw, style, CultureInfo.InvariantCulture, out value)
               || decimal.TryParse(raw.Replace(',', '.'), style, CultureInfo.InvariantCulture, out value)
               || decimal.TryParse(raw.Replace('.', ','), style, CultureInfo.GetCultureInfo("ru-RU"), out value);
    }
}

