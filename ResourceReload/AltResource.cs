using System.Text.Json;
using AltV.Net;
using ResourceReloader;

namespace ResourceReload;

public class AltResource : Resource
{
    private List<ResourceWatcher> resourceWatchers = [];
    
    public override void OnStart()
    {
        var rawConfig = File.ReadAllText("resources/ResourceLoader/config.json");
        var config = JsonSerializer.Deserialize<ResourceLoaderConfig>(rawConfig, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (config is null)
        {
            throw new ApplicationException("Failed to parse ResourceLoader config.json");
        } 

        foreach (var resource in config.Resources)
        {
            resourceWatchers.Add(new ResourceWatcher(resource));
        }
        
        foreach (var resourceWatcher in resourceWatchers)
        {
            resourceWatcher.Start();
        }
    }

    public override void OnStop()
    {
        foreach (var resourceWatcher in resourceWatchers)
        {
            resourceWatcher.Dispose();
        }
    }
}