using DavidHome.Optimizely.MultiSite;
using DavidHome.Optimizely.MultiSite.Options;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection.Extensions;
// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class MultiSiteServiceCollectionExtensions
{
    public static IServiceCollection AddDavidHomeMultiSiteRouting(this IServiceCollection services)
    {
        services
            .ConfigureOptions<ConfigureMultiSiteMvcOptions>()
            .ConfigureOptions<ConfigureMultiSiteRazorViewEngineOptions>()
            .TryAddEnumerable(ServiceDescriptor.Singleton<MatcherPolicy, MultiSiteMatcher>());

        return services;
    }
}