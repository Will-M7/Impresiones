using Impresiones.Application.Configuration;
using Impresiones.Infrastructure.Configuration;

namespace Impresiones.Infrastructure.Tests.Configuration;

public sealed class DataPathProviderTests
{
    [Fact]
    public void GetPaths_ResolvesAllExpectedDirectories()
    {
        using var temp = TempDirectory.Create();
        var options = new DataPathOptions { RootPath = temp.RootPath };

        var paths = new DataPathProvider(options).GetPaths();

        Assert.Equal(Path.GetFullPath(temp.RootPath), paths.Root);
        Assert.Equal(Path.Combine(paths.Root, "Inbox"), paths.Inbox);
        Assert.Equal(Path.Combine(paths.Root, "Processing"), paths.Processing);
        Assert.Equal(Path.Combine(paths.Root, "Printed"), paths.Printed);
        Assert.Equal(Path.Combine(paths.Root, "Discriminated"), paths.Discriminated);
        Assert.Equal(Path.Combine(paths.Root, "Previews"), paths.Previews);
        Assert.Equal(Path.Combine(paths.Root, "Temp"), paths.Temp);
        Assert.Equal(Path.Combine(paths.Root, "Logs"), paths.Logs);
        Assert.Equal(Path.Combine(paths.Root, "Database"), paths.Database);
    }

    [Fact]
    public void GetPaths_NormalizesRootPath()
    {
        using var temp = TempDirectory.Create();
        var rootWithCurrentSegment = Path.Combine(temp.RootPath, ".");

        var paths = new DataPathProvider(new DataPathOptions { RootPath = rootWithCurrentSegment }).GetPaths();

        Assert.Equal(Path.GetFullPath(temp.RootPath), paths.Root);
    }

    [Fact]
    public void GetPaths_ChangingRootPathChangesAllResolvedPaths()
    {
        using var first = TempDirectory.Create();
        using var second = TempDirectory.Create();

        var firstPaths = new DataPathProvider(new DataPathOptions { RootPath = first.RootPath }).GetPaths();
        var secondPaths = new DataPathProvider(new DataPathOptions { RootPath = second.RootPath }).GetPaths();

        Assert.NotEqual(firstPaths.Root, secondPaths.Root);
        Assert.StartsWith(secondPaths.Root, secondPaths.Inbox, StringComparison.OrdinalIgnoreCase);
        Assert.StartsWith(secondPaths.Root, secondPaths.Processing, StringComparison.OrdinalIgnoreCase);
        Assert.StartsWith(secondPaths.Root, secondPaths.Printed, StringComparison.OrdinalIgnoreCase);
        Assert.StartsWith(secondPaths.Root, secondPaths.Discriminated, StringComparison.OrdinalIgnoreCase);
        Assert.StartsWith(secondPaths.Root, secondPaths.Previews, StringComparison.OrdinalIgnoreCase);
        Assert.StartsWith(secondPaths.Root, secondPaths.Temp, StringComparison.OrdinalIgnoreCase);
        Assert.StartsWith(secondPaths.Root, secondPaths.Logs, StringComparison.OrdinalIgnoreCase);
        Assert.StartsWith(secondPaths.Root, secondPaths.Database, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void GetPaths_RejectsEmptyFolderName()
    {
        using var temp = TempDirectory.Create();
        var options = new DataPathOptions
        {
            RootPath = temp.RootPath,
            Inbox = " "
        };

        var ex = Assert.Throws<DataPathConfigurationException>(() => new DataPathProvider(options));
        Assert.Contains("Inbox", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void GetPaths_RejectsAbsoluteFolderName()
    {
        using var temp = TempDirectory.Create();
        var options = new DataPathOptions
        {
            RootPath = temp.RootPath,
            Inbox = Path.Combine(Path.GetPathRoot(temp.RootPath) ?? "C:\\", "Outside")
        };

        var ex = Assert.Throws<DataPathConfigurationException>(() => new DataPathProvider(options));
        Assert.Contains("relativo", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void GetPaths_RejectsParentTraversal()
    {
        using var temp = TempDirectory.Create();
        var options = new DataPathOptions
        {
            RootPath = temp.RootPath,
            Inbox = Path.Combine("Inbox", "..", "Outside")
        };

        var ex = Assert.Throws<DataPathConfigurationException>(() => new DataPathProvider(options));
        Assert.Contains("..", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void GetPaths_DoesNotRequireRootToExist()
    {
        using var temp = TempDirectory.Create();
        var missingRoot = Path.Combine(temp.RootPath, "MissingRoot");

        var paths = new DataPathProvider(new DataPathOptions { RootPath = missingRoot }).GetPaths();

        Assert.Equal(Path.GetFullPath(missingRoot), paths.Root);
        Assert.False(Directory.Exists(missingRoot));
    }

    [Fact]
    public void GetPaths_ConvertsInvalidRootPathToConfigurationException()
    {
        using var temp = TempDirectory.Create();
        var expectedFolder = Path.Combine(temp.RootPath, "Inbox");
        var options = new DataPathOptions
        {
            RootPath = temp.RootPath + "\0"
        };

        var ex = Assert.Throws<DataPathConfigurationException>(() => new DataPathProvider(options));

        Assert.Contains("RootPath", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.NotNull(ex.InnerException);
        Assert.False(Directory.Exists(expectedFolder));
    }

    [Fact]
    public void GetPaths_ConvertsInvalidChildPathToConfigurationException()
    {
        using var temp = TempDirectory.Create();
        var options = new DataPathOptions
        {
            RootPath = temp.RootPath,
            Inbox = "Inbox\0"
        };

        var ex = Assert.Throws<DataPathConfigurationException>(() => new DataPathProvider(options));

        Assert.Contains("Inbox", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.NotNull(ex.InnerException);
        Assert.Empty(Directory.GetDirectories(temp.RootPath));
    }

    [Fact]
    public void GetPaths_InvalidChildPathDoesNotCreatePartialDirectories()
    {
        using var temp = TempDirectory.Create();
        var options = new DataPathOptions
        {
            RootPath = temp.RootPath,
            Processing = "Processing\0"
        };

        Assert.Throws<DataPathConfigurationException>(() => new DataPathProvider(options));
        Assert.Empty(Directory.GetDirectories(temp.RootPath));
    }
}
