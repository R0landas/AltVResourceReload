using AltV.Net;

namespace ResourceReload;

public class ResourceWatcher : IDisposable
{
    private readonly FileSystemWatcher fileSystemWatcher;
    private readonly WatchedResource resource;
    private readonly Timer reloadTimer;

    public ResourceWatcher(WatchedResource resource)
    {
        this.resource = resource;
        
        fileSystemWatcher = new FileSystemWatcher(resource.SourceDirectory)
        {
            NotifyFilter = NotifyFilters.LastWrite,
            IncludeSubdirectories = true,
            Filter = "*.*",
            EnableRaisingEvents = true
        };
        
        reloadTimer = new Timer(OnReloadResource, null, TimeSpan.Zero, TimeSpan.FromSeconds(2));
    }

    private bool resourceNeedsReload;
    
    private void OnReloadResource(object? state)
    {
        if (!resourceNeedsReload)
        {
            return;
        }
        
        foreach (var player in Alt.GetAllPlayers())
        {
            player.Kick("Reloading resource...");
        }
        
        resourceNeedsReload = false;

        Alt.StopResource(resource.Name);
        while (Alt.GetResource(resource.Name) is not null)
        {
            Alt.Log($"Waiting for resource {resource.Name} to stop");
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }
        
        var resourceCopier = new ResourceFileCopier();
        Alt.Log($"Copying resource {resource.Name} files...");
        resourceCopier.CopyResourceFiles(resource);
        
        Alt.StartResource(resource.Name);
        Alt.Log($"Resource {resource.Name} restarted");
    }


    public void Start()
    {
        Alt.Log($"Resource watcher for resource {resource.Name} started");
        fileSystemWatcher.Changed += OnFilesUpdated;
    }

    private void OnFilesUpdated(object sender, FileSystemEventArgs e)
    {
        if (resourceNeedsReload)
        {
            return;
        }
        
        Alt.Log($"Resource {resource.Name} changed");
        resourceNeedsReload = true;
    }

    public void Dispose()
    {
        fileSystemWatcher.Dispose();
        reloadTimer.Dispose();
        GC.SuppressFinalize(this);
    }
}