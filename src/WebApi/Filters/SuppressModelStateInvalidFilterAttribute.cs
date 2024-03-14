using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace March2024BackendTask.WebApi.Filters;

[AttributeUsage(AttributeTargets.Method)]
public class SuppressModelStateInvalidFilterAttribute : Attribute, IActionModelConvention
{
    private static readonly Type? ModelStateInvalidFilterFactory = typeof(ModelStateInvalidFilter).Assembly.GetType
        ("Microsoft.AspNetCore.Mvc.Infrastructure.ModelStateInvalidFilterFactory");

    public void Apply(ActionModel action)
    {
        var filter = action.Filters.FirstOrDefault
            (f => f is ModelStateInvalidFilter || f.GetType() == ModelStateInvalidFilterFactory);
        if (filter != null) action.Filters.Remove(filter);
    }
}