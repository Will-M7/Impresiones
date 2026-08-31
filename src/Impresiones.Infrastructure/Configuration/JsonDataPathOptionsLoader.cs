using System.Text.Json;
using Impresiones.Application.Configuration;

namespace Impresiones.Infrastructure.Configuration;

public sealed class JsonDataPathOptionsLoader : IDataPathOptionsLoader
{
    private const string SectionName = "DataPaths";

    private readonly string configurationPath;

    public JsonDataPathOptionsLoader(string configurationPath)
    {
        this.configurationPath = configurationPath;
    }

    public DataPathOptions Load()
    {
        if (string.IsNullOrWhiteSpace(configurationPath))
        {
            throw new DataPathConfigurationException("La ruta de configuracion no puede estar vacia.");
        }

        if (!File.Exists(configurationPath))
        {
            throw new DataPathConfigurationException($"No se encontro el archivo de configuracion requerido: {Path.GetFileName(configurationPath)}.");
        }

        try
        {
            using var stream = File.OpenRead(configurationPath);
            using var document = JsonDocument.Parse(stream);

            if (!document.RootElement.TryGetProperty(SectionName, out var section))
            {
                throw new DataPathConfigurationException($"No se encontro la seccion de configuracion '{SectionName}'.");
            }

            var options = section.Deserialize<DataPathOptions>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return options ?? throw new DataPathConfigurationException($"La seccion de configuracion '{SectionName}' no es valida.");
        }
        catch (JsonException ex)
        {
            throw new DataPathConfigurationException("El archivo de configuracion no contiene JSON valido.", ex);
        }
        catch (IOException ex)
        {
            throw new DataPathConfigurationException("No se pudo leer el archivo de configuracion.", ex);
        }
    }
}
