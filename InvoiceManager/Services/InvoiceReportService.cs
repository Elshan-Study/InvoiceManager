using InvoiceManager.Data;
using InvoiceManager.DTOs.ReportDtos;
using InvoiceManager.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InvoiceManager.Services;

public class InvoiceReportService : IInvoiceReportService
{
    private readonly TaskFlowDbContext _context;

    public InvoiceReportService(TaskFlowDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CustomerStatDto>> GetCustomerStatsAsync(
        DateTimeOffset? startFrom,
        DateTimeOffset? endTo)
    {
        var query = _context.Invoices
            .Where(i => i.DeletedAt == null);

        if (startFrom.HasValue)
            query = query.Where(i => i.StartDate >= startFrom.Value);

        if (endTo.HasValue)
            query = query.Where(i => i.EndDate <= endTo.Value);

        var result = await query
            .Join(_context.Customers,
                  i => i.CustomerId,
                  c => c.Id,
                  (i, c) => new { Invoice = i, CustomerName = c.Name })
            .GroupBy(x => new { x.Invoice.CustomerId, x.CustomerName })
            .Select(g => new CustomerStatDto
            {
                CustomerId = g.Key.CustomerId,
                CustomerName = g.Key.CustomerName,
                InvoiceCount = g.Count(),
                TotalSum = g.Sum(x => x.Invoice.TotalSum)
            })
            .OrderByDescending(x => x.TotalSum)
            .ToListAsync();

        return result;
    }

    public async Task<IEnumerable<WorkStatDto>> GetWorkStatsAsync(
        DateTimeOffset? startFrom,
        DateTimeOffset? endTo)
    {
        var query = _context.Invoices
            .Where(i => i.DeletedAt == null);

        if (startFrom.HasValue)
            query = query.Where(i => i.StartDate >= startFrom.Value);

        if (endTo.HasValue)
            query = query.Where(i => i.EndDate <= endTo.Value);

        var invoiceIds = query.Select(i => i.Id);

        var result = await _context.InvoiceRows
            .Where(r => invoiceIds.Contains(r.InvoiceId))
            .GroupBy(r => r.Service)
            .Select(g => new WorkStatDto
            {
                WorkName = g.Key,
                InvoiceCount = g.Select(r => r.InvoiceId).Distinct().Count(),
                TotalSum = g.Sum(r => r.Sum)
            })
            .OrderByDescending(x => x.TotalSum)
            .ToListAsync();

        return result;
    }

    public async Task<IEnumerable<InvoiceStatusStatDto>> GetStatusStatsAsync(
        DateTimeOffset? startFrom,
        DateTimeOffset? endTo)
    {
        var query = _context.Invoices
            .Where(i => i.DeletedAt == null);

        if (startFrom.HasValue)
            query = query.Where(i => i.StartDate >= startFrom.Value);

        if (endTo.HasValue)
            query = query.Where(i => i.EndDate <= endTo.Value);

        var result = await query
            .GroupBy(i => i.Status)
            .Select(g => new InvoiceStatusStatDto
            {
                Status = g.Key,
                InvoiceCount = g.Count()
            })
            .OrderBy(x => x.Status)
            .ToListAsync();

        return result;
    }
}
