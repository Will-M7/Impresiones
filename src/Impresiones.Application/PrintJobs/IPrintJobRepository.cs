using Impresiones.Domain.Entities;

namespace Impresiones.Application.PrintJobs;

public interface IPrintJobRepository
{
    Task<PrintJob?> GetByIdAsync(string printJobId, CancellationToken cancellationToken);

    Task SaveAsync(PrintJob printJob, CancellationToken cancellationToken);
}
