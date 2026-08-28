namespace Impresiones.Application.Tests;

public class ApplicationAssemblyTests
{
    [Fact]
    public void ApplicationAssemblyMarker_IsAvailable()
    {
        Assert.Equal("Impresiones.Application", typeof(ApplicationAssemblyMarker).Namespace);
    }
}
