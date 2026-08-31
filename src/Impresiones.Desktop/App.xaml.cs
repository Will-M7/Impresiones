using System.IO;
using System.Windows;
using Impresiones.Infrastructure.Configuration;

namespace Impresiones.Desktop;

public partial class App : System.Windows.Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        try
        {
            var configurationPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            var optionsLoader = new JsonDataPathOptionsLoader(configurationPath);
            var pathProvider = new DataPathProvider(optionsLoader.Load());
            var directoryInitializer = new DataDirectoryInitializer(pathProvider);

            directoryInitializer.EnsureCreated();
        }
        catch (DataPathConfigurationException ex)
        {
            MessageBox.Show(
                ex.Message,
                "Configuracion invalida",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            Shutdown(1);
            return;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            MessageBox.Show(
                "No se pudieron preparar las carpetas de datos configuradas.",
                "Error de inicializacion",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            Shutdown(1);
            return;
        }

        base.OnStartup(e);
    }
}
