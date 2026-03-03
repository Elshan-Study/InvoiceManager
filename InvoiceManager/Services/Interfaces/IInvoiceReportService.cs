using InvoiceManager.DTOs.ReportDtos;

namespace InvoiceManager.Services.Interfaces;

public interface IInvoiceReportService
{
    Task<IEnumerable<CustomerStatDto>> GetCustomerStatsAsync(
        DateTimeOffset? startFrom,
        DateTimeOffset? endTo);

    Task<IEnumerable<WorkStatDto>> GetWorkStatsAsync(
        DateTimeOffset? startFrom,
        DateTimeOffset? endTo);

    Task<IEnumerable<InvoiceStatusStatDto>> GetStatusStatsAsync(
        DateTimeOffset? startFrom,
        DateTimeOffset? endTo);
}
