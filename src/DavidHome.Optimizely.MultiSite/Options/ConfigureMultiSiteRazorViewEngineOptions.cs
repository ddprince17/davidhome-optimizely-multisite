using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;

namespace DavidHome.Optimizely.MultiSite.Options;

public class ConfigureMultiSiteRazorViewEngineOptions : IConfigureOptions<RazorViewEngineOptions>
{
    public void Configure(RazorViewEngineOptions options)
    {
        options.ViewLocationExpanders.Add(new MultiSiteViewLocationExpander());
    }
}