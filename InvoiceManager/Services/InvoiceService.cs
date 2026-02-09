using AutoMapper;
using InvoiceManager.Common;
using InvoiceManager.Data;
using InvoiceManager.DTOs.InvoiceDto;
using InvoiceManager.Models;
using InvoiceManager.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InvoiceManager.Services;

public class InvoiceService : IInvoiceService
{
    private readonly TaskFlowDbContext _context;
    private readonly IMapper _mapper;

    public InvoiceService(TaskFlowDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<InvoiceResponseDto> CreateAsync(CreateInvoiceDto dto)
    {
        // Проверка существования клиента
        var customerExists = await _context.Customers.AnyAsync(c => c.Id == dto.CustomerId);
        if (!customerExists) throw new KeyNotFoundException($"Customer with Id {dto.CustomerId} not found.");

        var invoice = _mapper.Map<Invoice>(dto);

        var now = DateTimeOffset.UtcNow; //add
        invoice.CreatedAt = now; //add
        invoice.UpdatedAt = now; //add

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        decimal total = 0m;

        foreach (var rowDto in dto.Rows)
        {
            var row = _mapper.Map<InvoiceRow>(rowDto);
            row.InvoiceId = invoice.Id;
            row.Sum = Decimal.Round(row.Quantity * row.Rate, 2, MidpointRounding.AwayFromZero); //add

            total += row.Sum; //add
            _context.InvoiceRows.Add(row);
        }

        invoice.TotalSum = Decimal.Round(total, 2, MidpointRounding.AwayFromZero); //add
        invoice.UpdatedAt = DateTimeOffset.UtcNow; //add

        await _context.SaveChangesAsync();

        await _context.Entry(invoice).Collection(i => i.Rows).LoadAsync();

        return _mapper.Map<InvoiceResponseDto>(invoice);
    }

    public async Task<InvoiceResponseDto?> UpdateAsync(int id, UpdateInvoiceDto dto)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Rows)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice is null) return null;
        if (invoice.Status == InvoiceStatus.Sent)
            return null;

        // Обновляем заголовок
        invoice.StartDate = dto.StartDate;
        invoice.EndDate = dto.EndDate;
        invoice.Comment = dto.Comment;

        if (dto.Rows != null) //add
        {
            _context.InvoiceRows.RemoveRange(invoice.Rows);

            decimal total = 0m;
            var newRows = new List<InvoiceRow>();
            foreach (var rDto in dto.Rows)
            {
                var row = _mapper.Map<InvoiceRow>(rDto);
                row.InvoiceId = invoice.Id;
                row.Sum = Decimal.Round(row.Quantity * row.Rate, 2, MidpointRounding.AwayFromZero);
                total += row.Sum;
                newRows.Add(row);
            }

            invoice.TotalSum = Decimal.Round(total, 2, MidpointRounding.AwayFromZero);
            invoice.Rows = newRows;
            _context.InvoiceRows.AddRange(newRows);
        }
        else
        {
            invoice.TotalSum = invoice.Rows.Sum(r => r.Sum);
        }

        invoice.UpdatedAt = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync();

        return _mapper.Map<InvoiceResponseDto>(invoice);
    }

    public async Task<bool> ChangeStatusAsync(int id, InvoiceStatus status)
    {
        var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.Id == id); //changed
        if (invoice is null) return false;

        invoice.Status = status;
        invoice.UpdatedAt = DateTimeOffset.UtcNow; //added

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteSoftAsync(int id)
    {
        var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.Id == id); //changed
        if (invoice is null) return false;

        invoice.DeletedAt = DateTimeOffset.UtcNow; 
        invoice.UpdatedAt = DateTimeOffset.UtcNow; //add
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteHardAsync(int id)
    {
        var invoice = await _context.Invoices.IgnoreQueryFilters().FirstOrDefaultAsync(i => i.Id == id);
        if (invoice is null) return false;
        if (invoice.Status == InvoiceStatus.Sent) return false;

        _context.Invoices.Remove(invoice);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<InvoiceResponseDto>> GetAllAsync()
    {
        var invoices = await _context.Invoices
            .Include(i => i.Rows)
            .ToListAsync();

        return _mapper.Map<IEnumerable<InvoiceResponseDto>>(invoices);
    }

    public async Task<InvoiceResponseDto?> GetByIdAsync(int id)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Rows)
            .FirstOrDefaultAsync(i => i.Id == id); 

        return invoice is null ? null : _mapper.Map<InvoiceResponseDto>(invoice);
    }

    public async Task<PagedResult<InvoiceResponseDto>> GetPagedAsync(
    int page = 1,
    int pageSize = 10,
    int? customerId = null,
    InvoiceStatus? status = null,
    DateTimeOffset? startFrom = null,
    DateTimeOffset? endTo = null,
    string? sortBy = "Id",
    bool ascending = true)
    {
        var query = _context.Invoices
                            .Include(i => i.Rows)
                            .Where(i => i.DeletedAt == null)
                            .AsQueryable();

        if (customerId.HasValue)
            query = query.Where(i => i.CustomerId == customerId.Value);

        if (status.HasValue)
            query = query.Where(i => i.Status == status.Value);

        if (startFrom.HasValue)
            query = query.Where(i => i.StartDate >= startFrom.Value);

        if (endTo.HasValue)
            query = query.Where(i => i.EndDate <= endTo.Value);

        query = sortBy?.ToLower() switch
        {
            "startdate" => ascending ? query.OrderBy(i => i.StartDate) : query.OrderByDescending(i => i.StartDate),
            "enddate" => ascending ? query.OrderBy(i => i.EndDate) : query.OrderByDescending(i => i.EndDate),
            "status" => ascending ? query.OrderBy(i => i.Status) : query.OrderByDescending(i => i.Status),
            _ => ascending ? query.OrderBy(i => i.Id) : query.OrderByDescending(i => i.Id)
        };

        var totalCount = await query.CountAsync();
        var items = await query
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();

        var mapped = _mapper.Map<IEnumerable<InvoiceResponseDto>>(items);

        return PagedResult<InvoiceResponseDto>.Create(mapped, page, pageSize, totalCount);
    }

}
