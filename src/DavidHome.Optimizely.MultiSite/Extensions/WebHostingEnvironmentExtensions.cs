using DavidHome.Optimizely.MultiSite;
using Microsoft.Extensions.FileProviders;
// ReSharper disable CheckNamespace

namespace Microsoft.AspNetCore.Hosting;

public static class WebHostingEnvironmentExtensions
{
    public static IWebHostEnvironment AddDavidHomeMultiSiteFileProvider(this IWebHostEnvironment webHostEnvironment)
    {
        var webRootFileProvider = webHostEnvironment.WebRootFileProvider;

        if (webRootFileProvider is CompositeFileProvider compositeFileProvider)
        {
            var physicalFileProvider = compositeFileProvider.FileProviders.FirstOrDefault(provider =>
                provider is PhysicalFileProvider physicalFileProvider && physicalFileProvider.Root.Contains(webHostEnvironment.WebRootPath));

            if (physicalFileProvider != null)
            {
                webHostEnvironment.WebRootFileProvider = new CompositeFileProvider(compositeFileProvider.FileProviders.Concat([new MultiSiteAssetsFileProvider(physicalFileProvider)]));

                return webHostEnvironment;
            }
        }

        webHostEnvironment.WebRootFileProvider = new CompositeFileProvider(webRootFileProvider, new MultiSiteAssetsFileProvider(webRootFileProvider));

        return webHostEnvironment;
    }
}