using Impresiones.Application.Configuration;

namespace Impresiones.Infrastructure.Configuration;

public sealed class DataDirectoryInitializer : IDataDirectoryInitializer
{
    private readonly IDataPathProvider pathProvider;

    public DataDirectoryInitializer(IDataPathProvider pathProvider)
    {
        this.pathProvider = pathProvider;
    }

    public void EnsureCreated()
    {
        var paths = pathProvider.GetPaths();

        foreach (var directory in paths.RequiredDirectories)
        {
            Directory.CreateDirectory(directory);
        }
    }
}
