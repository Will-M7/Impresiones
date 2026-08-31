namespace Impresiones.Application.Configuration;

public sealed record DataPaths(
    string Root,
    string Inbox,
    string Processing,
    string Printed,
    string Discriminated,
    string Previews,
    string Temp,
    string Logs,
    string Database)
{
    public IReadOnlyList<string> RequiredDirectories { get; } =
    [
        Root,
        Inbox,
        Processing,
        Printed,
        Discriminated,
        Previews,
        Temp,
        Logs,
        Database
    ];
}
