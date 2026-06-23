using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace InventorySystem.Services;

/// <summary>
/// Defines a contract for rendering a Razor view to its HTML string representation using a specified model.
/// </summary>
/// <remarks>Implementations of this interface are typically used to generate HTML content from Razor views
/// outside of the standard MVC controller pipeline, such as for email templates or background processing. Thread safety
/// and performance characteristics may vary depending on the implementation.</remarks>
public interface IRazorViewToStringRenderer
{
    Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model);
}
/// <summary>
/// Provides functionality to render Razor views to strings using a specified model.
/// </summary>
/// <remarks>This class is typically used in scenarios where you need to generate HTML from Razor views outside of
/// the standard MVC request pipeline, such as for email templates or background processing. It relies on ASP.NET Core's
/// view engine and service provider to resolve views and dependencies.</remarks>
public class RazorViewToStringRenderer(ICompositeViewEngine viewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider) : IRazorViewToStringRenderer
{
    private readonly ICompositeViewEngine _viewEngine = viewEngine;
    private readonly ITempDataProvider _tempDataProvider = tempDataProvider;
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    public async Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model)
    {
        var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());

        ViewEngineResult viewResult;

        // If full path is provided, use GetView
        if (viewName.StartsWith("~") || viewName.StartsWith("/"))
        {
            viewResult = _viewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: false);
        }
        else
        {
            viewResult = _viewEngine.FindView(actionContext, viewName, isMainPage: false);
        }

        if (!viewResult.Success)
        {
            throw new InvalidOperationException($"View '{viewName}' not found.");
        }

        await using var sw = new StringWriter();

        var viewContext = new ViewContext(
            actionContext,
            viewResult.View,
            new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            },
            new TempDataDictionary(httpContext, _tempDataProvider),
            sw,
            new HtmlHelperOptions()
        );

        await viewResult.View.RenderAsync(viewContext);
        return sw.ToString();
    }
}