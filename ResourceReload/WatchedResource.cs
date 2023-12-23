namespace ResourceReload;

public record WatchedResource
{
    public string Name { get; set; }
    public string SourceDirectory { get; set; }
    public string Directory { get; set; }
}