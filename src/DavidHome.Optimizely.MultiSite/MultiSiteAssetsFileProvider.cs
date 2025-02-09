using EPiServer.Web;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace DavidHome.Optimizely.MultiSite;

/// <summary>
/// Provides a file provider that prefixes the path with the current site name.
/// This will allow you to serve static assets from a virtual location, as if they were physically located in the root of wwwroot.
/// </summary>
public class MultiSiteAssetsFileProvider : IFileProvider
{
    private readonly IFileProvider _fileProvider;

    public MultiSiteAssetsFileProvider(IFileProvider fileProvider)
    {
        _fileProvider = fileProvider;
    }

    public IDirectoryContents GetDirectoryContents(string subpath)
    {
        var sitePath = GetSitePath(subpath);

        return sitePath != null ? _fileProvider.GetDirectoryContents(sitePath) : NotFoundDirectoryContents.Singleton;
    }

    public IFileInfo GetFileInfo(string subpath)
    {
        var sitePath = GetSitePath(subpath);

        return sitePath != null ? _fileProvider.GetFileInfo(sitePath) : new NotFoundFileInfo(subpath);
    }

    public IChangeToken Watch(string filter)
    {
        var siteFilter = GetSitePath(filter);
        
        return siteFilter != null ? _fileProvider.Watch(siteFilter) : NullChangeToken.Singleton;
    }
    
    private static string? GetSitePath(string subPath)
    {
        var siteName = SiteDefinition.Current?.Name;

        return !string.IsNullOrEmpty(siteName) ? $"/{siteName.ToLowerInvariant()}{subPath}" : null;
    }
}