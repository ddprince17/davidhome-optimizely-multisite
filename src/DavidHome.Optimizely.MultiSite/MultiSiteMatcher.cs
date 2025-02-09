using EPiServer;
using EPiServer.Core;
using EPiServer.Web;
using EPiServer.Web.Routing.Matching;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Matching;

namespace DavidHome.Optimizely.MultiSite;

/// <summary>
/// ASP.NET Core routing policy that sets the site name and page type name route values.
/// </summary>
public class MultiSiteMatcher : MatcherPolicy, IEndpointSelectorPolicy
{
    public const string SiteNameRouteKey = "siteName";
    public const string PageTypeNameRouteKey = "pageTypeName";

    // The order is set to run just after the ContentMatcherPolicy.
    public override int Order => 2147483548;

    public bool AppliesToEndpoints(IReadOnlyList<Endpoint> endpoints)
    {
        return ContainsDynamicEndpoints(endpoints);
    }

    public Task ApplyAsync(HttpContext httpContext, CandidateSet candidates)
    {
        var contentRouteFeature = httpContext.Features.Get<IContentRouteFeature>();
        var siteDefinition = contentRouteFeature?.RoutedContentData?.MatchedHost?.Site;
        var routedContent = contentRouteFeature?.RoutedContentData?.Content;

        SetSiteNameRouteValue(httpContext, siteDefinition);
        SetTypeNameRouteValue(httpContext, routedContent);

        return Task.CompletedTask;
    }

    private static void SetSiteNameRouteValue(HttpContext httpContext, SiteDefinition? siteDefinition)
    {
        if (siteDefinition != null && !SiteDefinition.Empty.Equals(siteDefinition))
        {
            httpContext.Request.RouteValues[SiteNameRouteKey] = siteDefinition.Name;
        }
    }

    private static void SetTypeNameRouteValue(HttpContext httpContext, IContent? routedContent)
    {
        if (routedContent == null)
        {
            return;
        }

        var pageTypeName = routedContent.GetOriginalType().Name;

        if (!string.IsNullOrEmpty(pageTypeName))
        {
            httpContext.Request.RouteValues[PageTypeNameRouteKey] = pageTypeName;
        }
    }
}