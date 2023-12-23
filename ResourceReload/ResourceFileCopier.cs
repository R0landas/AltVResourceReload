namespace ResourceReload;

public class ResourceFileCopier
{
    public void CopyResourceFiles(WatchedResource resource)
    {
        var filesToCopy = Directory.GetFiles(resource.SourceDirectory);
        foreach (var file in filesToCopy)
        {
            File.Copy(file, Path.Combine(resource.Directory, Path.GetFileName(file)), overwrite: true);
        }
    }
}