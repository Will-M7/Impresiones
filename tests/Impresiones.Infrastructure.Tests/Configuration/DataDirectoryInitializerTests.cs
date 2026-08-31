using Impresiones.Application.Configuration;
using Impresiones.Infrastructure.Configuration;

namespace Impresiones.Infrastructure.Tests.Configuration;

public sealed class DataDirectoryInitializerTests
{
    [Fact]
    public void EnsureCreated_CreatesRootAndAllExpectedDirectories()
    {
        using var temp = TempDirectory.Create();
        var root = Path.Combine(temp.RootPath, "DataRoot");
        var paths = new DataPathProvider(new DataPathOptions { RootPath = root });
        var initializer = new DataDirectoryInitializer(paths);

        initializer.EnsureCreated();

        var resolved = paths.GetPaths();
        Assert.True(Directory.Exists(resolved.Root));
        Assert.True(Directory.Exists(resolved.Inbox));
        Assert.True(Directory.Exists(resolved.Processing));
        Assert.True(Directory.Exists(resolved.Printed));
        Assert.True(Directory.Exists(resolved.Discriminated));
        Assert.True(Directory.Exists(resolved.Previews));
        Assert.True(Directory.Exists(resolved.Temp));
        Assert.True(Directory.Exists(resolved.Logs));
        Assert.True(Directory.Exists(resolved.Database));
    }

    [Fact]
    public void EnsureCreated_CanRunTwice()
    {
        using var temp = TempDirectory.Create();
        var initializer = new DataDirectoryInitializer(
            new DataPathProvider(new DataPathOptions { RootPath = temp.RootPath }));

        initializer.EnsureCreated();
        initializer.EnsureCreated();

        Assert.True(Directory.Exists(Path.Combine(temp.RootPath, "Inbox")));
    }

    [Fact]
    public void EnsureCreated_DoesNotDeleteExistingFilesOrFolders()
    {
        using var temp = TempDirectory.Create();
        var existingFolder = Path.Combine(temp.RootPath, "ExistingFolder");
        var existingFile = Path.Combine(temp.RootPath, "existing-file.txt");
        Directory.CreateDirectory(existingFolder);
        File.WriteAllText(existingFile, "preserve");

        var initializer = new DataDirectoryInitializer(
            new DataPathProvider(new DataPathOptions { RootPath = temp.RootPath }));

        initializer.EnsureCreated();

        Assert.True(Directory.Exists(existingFolder));
        Assert.True(File.Exists(existingFile));
        Assert.Equal("preserve", File.ReadAllText(existingFile));
    }

    [Fact]
    public void EnsureCreated_DoesNotCreateAdditionalDirectories()
    {
        using var temp = TempDirectory.Create();
        var initializer = new DataDirectoryInitializer(
            new DataPathProvider(new DataPathOptions { RootPath = temp.RootPath }));

        initializer.EnsureCreated();

        var directoryNames = Directory.GetDirectories(temp.RootPath)
            .Select(Path.GetFileName)
            .Where(name => name is not null)
            .Select(name => name!)
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            [
                "Database",
                "Discriminated",
                "Inbox",
                "Logs",
                "Previews",
                "Printed",
                "Processing",
                "Temp"
            ],
            directoryNames);
    }
}
