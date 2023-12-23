using ResourceReload;

namespace ResourceReloader;

public class ResourceLoaderConfig
{
    public IEnumerable<WatchedResource> Resources { get; set; }
}