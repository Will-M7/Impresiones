namespace Impresiones.Infrastructure.Tests;

public class InfrastructureAssemblyTests
{
    [Fact]
    public void InfrastructureAssemblyMarker_IsAvailable()
    {
        Assert.Equal("Impresiones.Infrastructure", typeof(InfrastructureAssemblyMarker).Namespace);
    }
}
