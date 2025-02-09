using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using EPiServer.DataAbstraction.RuntimeModel.Internal;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DavidHome.Optimizely.MultiSite;

/// <summary>
/// Will register view locations for the current site if the site name is present in the route values.
/// </summary>
public class MultiSiteActionFilter : IAsyncActionFilter
{
    private static readonly ConcurrentDictionary<string, bool> SiteRegistrations = new(StringComparer.OrdinalIgnoreCase);

    private readonly IViewRegistrator _viewRegistrator;

    public MultiSiteActionFilter(IViewRegistrator viewRegistrator)
    {
        _viewRegistrator = viewRegistrator;
    }

    [SuppressMessage("ReSharper", "InvertIf")]
    public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // If this condition is met, this piece of code will ask Optimizely to register new templates which are specific to the running site. 
        // The OOB behavior doesn't take into account custom view location expanders with custom values, which affects the view resolution at runtime.
        if (context.HttpContext.Request.RouteValues.TryGetValue(MultiSiteMatcher.SiteNameRouteKey, out var name) && name is string siteName)
        {
            SiteRegistrations.TryGetValue(siteName, out var isRegistered);

            if (!isRegistered)
            {
                _viewRegistrator.RegisterViews(context.HttpContext);
                SiteRegistrations[siteName] = true;
            }
        }

        return next();
    }
}