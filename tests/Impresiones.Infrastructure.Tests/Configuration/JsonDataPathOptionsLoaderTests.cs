using Impresiones.Infrastructure.Configuration;

namespace Impresiones.Infrastructure.Tests.Configuration;

public sealed class JsonDataPathOptionsLoaderTests
{
    [Fact]
    public void Load_ReadsValidJsonConfiguration()
    {
        using var temp = TempDirectory.Create();
        var configuredRoot = Path.Combine(temp.RootPath, "ConfiguredRoot");
        var escapedRoot = configuredRoot.Replace("\\", "\\\\");
        var jsonPath = temp.WriteFile("appsettings.json", $$"""
            {
              "DataPaths": {
                "RootPath": "{{escapedRoot}}",
                "Inbox": "Inbox",
                "Processing": "Processing",
                "Printed": "Printed",
                "Discriminated": "Discriminated",
                "Previews": "Previews",
                "Temp": "Temp",
                "Logs": "Logs",
                "Database": "Database"
              }
            }
            """);

        var options = new JsonDataPathOptionsLoader(jsonPath).Load();

        Assert.Equal(configuredRoot, options.RootPath);
        Assert.Equal("Inbox", options.Inbox);
        Assert.Equal("Database", options.Database);
    }

    [Fact]
    public void Load_ThrowsClearErrorWhenJsonFileIsMissing()
    {
        using var temp = TempDirectory.Create();
        var missingPath = Path.Combine(temp.RootPath, "missing.json");

        var ex = Assert.Throws<DataPathConfigurationException>(() => new JsonDataPathOptionsLoader(missingPath).Load());

        Assert.Contains("No se encontro", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Load_ThrowsClearErrorWhenJsonIsInvalid()
    {
        using var temp = TempDirectory.Create();
        var jsonPath = temp.WriteFile("appsettings.json", "{ invalid json");

        var ex = Assert.Throws<DataPathConfigurationException>(() => new JsonDataPathOptionsLoader(jsonPath).Load());

        Assert.Contains("JSON valido", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Load_ThrowsClearErrorWhenDataPathsSectionIsMissing()
    {
        using var temp = TempDirectory.Create();
        var configuredRoot = Path.Combine(temp.RootPath, "ConfiguredRoot").Replace("\\", "\\\\");
        var jsonPath = temp.WriteFile("appsettings.json", $$"""
            {
              "Other": {
                "RootPath": "{{configuredRoot}}"
              }
            }
            """);

        var ex = Assert.Throws<DataPathConfigurationException>(() => new JsonDataPathOptionsLoader(jsonPath).Load());

        Assert.Contains("DataPaths", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}
