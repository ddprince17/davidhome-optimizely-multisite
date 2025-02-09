using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DavidHome.Optimizely.MultiSite.Options;

public class ConfigureMultiSiteMvcOptions : IConfigureOptions<MvcOptions>
{
    public void Configure(MvcOptions options)
    {
        options.Filters.Add<MultiSiteActionFilter>();
    }
}