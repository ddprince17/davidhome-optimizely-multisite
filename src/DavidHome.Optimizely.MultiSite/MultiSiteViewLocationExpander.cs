using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;

namespace DavidHome.Optimizely.MultiSite;

/// <summary>
/// This class is responsible for expanding the view locations based on the site name and page type name.
/// </summary>
public class MultiSiteViewLocationExpander : IViewLocationExpander
{
    private const string ViewsAndControllerFormat = "/Views/{1}/";
    private const string ViewsAndSharedFormat = "/Views/Shared/";
    private const string ActionAndViewFileExtension = "/{0}.cshtml";
    private const string BlockActionAndFileExtension = "/Blocks/{0}.cshtml";

    public void PopulateValues(ViewLocationExpanderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var siteName = context.ActionContext.HttpContext.GetRouteValue(MultiSiteMatcher.SiteNameRouteKey) as string;
        var pageTypeName = context.ActionContext.HttpContext.GetRouteValue(MultiSiteMatcher.PageTypeNameRouteKey) as string;

        context.Values[MultiSiteMatcher.SiteNameRouteKey] = siteName;
        context.Values[MultiSiteMatcher.PageTypeNameRouteKey] = pageTypeName;
    }

    public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(viewLocations);

        context.Values.TryGetValue(MultiSiteMatcher.SiteNameRouteKey, out var siteName);
        context.Values.TryGetValue(MultiSiteMatcher.PageTypeNameRouteKey, out var pageTypeName);

        return ExpandViewLocationsInternal(viewLocations, siteName, pageTypeName);
    }

    private static IEnumerable<string> ExpandViewLocationsInternal(IEnumerable<string> viewLocations, string? siteName, string? pageTypeName)
    {
        foreach (var location in viewLocations)
        {
            var hasSiteName = !string.IsNullOrEmpty(siteName);
            var hasPageTypeName = !string.IsNullOrEmpty(pageTypeName);

            if (location.Contains(ViewsAndSharedFormat, StringComparison.OrdinalIgnoreCase))
            {
                if (hasSiteName)
                {
                    var siteSharedLocation = location.Replace(ViewsAndSharedFormat, $"/Views/{siteName}/Shared/", StringComparison.OrdinalIgnoreCase);
                    
                    yield return siteSharedLocation;
                    yield return siteSharedLocation.Replace(ActionAndViewFileExtension, BlockActionAndFileExtension, StringComparison.OrdinalIgnoreCase);
                }
                
                // This will give a default location for all blocks, which is not the default under Optimizely CMS.
                yield return location.Replace(ActionAndViewFileExtension, BlockActionAndFileExtension, StringComparison.OrdinalIgnoreCase);
            }

            if (location.Contains(ViewsAndControllerFormat, StringComparison.OrdinalIgnoreCase))
            {
                if (hasSiteName && hasPageTypeName)
                {
                    yield return location.Replace(ViewsAndControllerFormat, $"/Views/{siteName}/{pageTypeName}/", StringComparison.OrdinalIgnoreCase);
                }

                if (hasSiteName)
                {
                    yield return location.Replace(ViewsAndControllerFormat, $"/Views/{siteName}/", StringComparison.OrdinalIgnoreCase);
                }

                if (hasPageTypeName)
                {
                    yield return location.Replace(ViewsAndControllerFormat, $"/Views/{pageTypeName}/", StringComparison.OrdinalIgnoreCase);
                }
            }

            yield return location;
        }
    }
}