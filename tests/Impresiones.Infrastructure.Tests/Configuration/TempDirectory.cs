namespace Impresiones.Infrastructure.Tests.Configuration;

internal sealed class TempDirectory : IDisposable
{
    private const string Prefix = "ImpresionesTests-";

    private TempDirectory(string rootPath)
    {
        RootPath = rootPath;
        Directory.CreateDirectory(RootPath);
    }

    public string RootPath { get; }

    public static TempDirectory Create()
    {
        var rootPath = Path.Combine(Path.GetTempPath(), Prefix + Guid.NewGuid().ToString("N"));

        return new TempDirectory(rootPath);
    }

    public string WriteFile(string fileName, string contents)
    {
        var filePath = Path.Combine(RootPath, fileName);
        File.WriteAllText(filePath, contents);

        return filePath;
    }

    public void Dispose()
    {
        if (!IsOwnedTestDirectory(RootPath))
        {
            return;
        }

        if (Directory.Exists(RootPath))
        {
            Directory.Delete(RootPath, recursive: true);
        }
    }

    private static bool IsOwnedTestDirectory(string path)
    {
        var fullPath = Path.GetFullPath(path);
        var tempPath = EnsureTrailingSeparator(Path.GetFullPath(Path.GetTempPath()));
        var directoryName = Path.GetFileName(fullPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

        return fullPath.StartsWith(tempPath, StringComparison.OrdinalIgnoreCase)
            && directoryName.StartsWith(Prefix, StringComparison.Ordinal);
    }

    private static string EnsureTrailingSeparator(string path)
    {
        if (Path.EndsInDirectorySeparator(path))
        {
            return path;
        }

        return path + Path.DirectorySeparatorChar;
    }
}
