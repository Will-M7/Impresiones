namespace Impresiones.Application.Configuration;

public sealed class DataPathOptions
{
    public string RootPath { get; init; } = string.Empty;

    public string Inbox { get; init; } = "Inbox";

    public string Processing { get; init; } = "Processing";

    public string Printed { get; init; } = "Printed";

    public string Discriminated { get; init; } = "Discriminated";

    public string Previews { get; init; } = "Previews";

    public string Temp { get; init; } = "Temp";

    public string Logs { get; init; } = "Logs";

    public string Database { get; init; } = "Database";
}
