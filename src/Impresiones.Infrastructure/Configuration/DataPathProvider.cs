using Impresiones.Application.Configuration;

namespace Impresiones.Infrastructure.Configuration;

public sealed class DataPathProvider : IDataPathProvider
{
    private static readonly StringComparer PathComparer = StringComparer.OrdinalIgnoreCase;

    private readonly DataPaths paths;

    public DataPathProvider(DataPathOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        paths = Resolve(options);
    }

    public DataPaths GetPaths() => paths;

    private static DataPaths Resolve(DataPathOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.RootPath))
        {
            throw new DataPathConfigurationException("DataPaths:RootPath debe tener un valor.");
        }

        var root = GetFullPath(options.RootPath.Trim(), "RootPath");

        return new DataPaths(
            root,
            ResolveChild(root, nameof(options.Inbox), options.Inbox),
            ResolveChild(root, nameof(options.Processing), options.Processing),
            ResolveChild(root, nameof(options.Printed), options.Printed),
            ResolveChild(root, nameof(options.Discriminated), options.Discriminated),
            ResolveChild(root, nameof(options.Previews), options.Previews),
            ResolveChild(root, nameof(options.Temp), options.Temp),
            ResolveChild(root, nameof(options.Logs), options.Logs),
            ResolveChild(root, nameof(options.Database), options.Database));
    }

    private static string ResolveChild(string root, string optionName, string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            throw new DataPathConfigurationException($"DataPaths:{optionName} debe tener un valor relativo.");
        }

        var trimmedPath = relativePath.Trim();

        if (Path.IsPathRooted(trimmedPath))
        {
            throw new DataPathConfigurationException($"DataPaths:{optionName} debe ser relativo a RootPath.");
        }

        if (ContainsParentTraversal(trimmedPath))
        {
            throw new DataPathConfigurationException($"DataPaths:{optionName} no puede contener segmentos '..'.");
        }

        var resolvedPath = GetFullPath(Path.Combine(root, trimmedPath), optionName);

        if (!IsInsideRoot(root, resolvedPath))
        {
            throw new DataPathConfigurationException($"DataPaths:{optionName} debe permanecer dentro de RootPath.");
        }

        return resolvedPath;
    }

    private static bool ContainsParentTraversal(string relativePath)
    {
        var segments = relativePath.Split(
            [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar],
            StringSplitOptions.RemoveEmptyEntries);

        return segments.Any(segment => segment == "..");
    }

    private static bool IsInsideRoot(string root, string path)
    {
        var normalizedRoot = EnsureTrailingSeparator(GetFullPath(root, "RootPath"));
        var normalizedPath = GetFullPath(path, "ResolvedPath");

        return normalizedPath.StartsWith(normalizedRoot, StringComparison.OrdinalIgnoreCase)
            && !PathComparer.Equals(
                normalizedPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                normalizedRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
    }

    private static string EnsureTrailingSeparator(string path)
    {
        if (Path.EndsInDirectorySeparator(path))
        {
            return path;
        }

        return path + Path.DirectorySeparatorChar;
    }

    private static string GetFullPath(string path, string optionName)
    {
        try
        {
            return Path.GetFullPath(path);
        }
        catch (ArgumentException ex)
        {
            throw CreateInvalidPathException(optionName, ex);
        }
        catch (PathTooLongException ex)
        {
            throw CreateInvalidPathException(optionName, ex);
        }
        catch (NotSupportedException ex)
        {
            throw CreateInvalidPathException(optionName, ex);
        }
    }

    private static DataPathConfigurationException CreateInvalidPathException(string optionName, Exception innerException)
    {
        var message = optionName == "RootPath"
            ? "DataPaths:RootPath no es una ruta valida."
            : $"DataPaths:{optionName} no se pudo resolver como una ruta valida.";

        return new DataPathConfigurationException(message, innerException);
    }
}
