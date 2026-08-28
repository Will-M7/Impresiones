namespace Impresiones.Domain.Tests;

public class DomainAssemblyTests
{
    [Fact]
    public void DomainAssemblyMarker_IsAvailable()
    {
        Assert.Equal("Impresiones.Domain", typeof(DomainAssemblyMarker).Namespace);
    }
}
