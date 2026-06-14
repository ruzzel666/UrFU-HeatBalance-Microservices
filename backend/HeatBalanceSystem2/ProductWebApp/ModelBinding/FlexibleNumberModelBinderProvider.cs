using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ProductWebApp.ModelBinding;

public sealed class FlexibleNumberModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        var t = context.Metadata.ModelType;
        var underlying = context.Metadata.UnderlyingOrModelType;

        if (underlying != typeof(double) &&
            underlying != typeof(float) &&
            underlying != typeof(decimal))
        {
            return null;
        }

        return new FlexibleNumberModelBinder(t);
    }
}

