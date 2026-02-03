using AutoMapper;
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
        var invoice = _mapper.Map<Invoice>(dto);

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        foreach (var rowDto in dto.Rows)
        {
            var row = _mapper.Map<InvoiceRow>(rowDto);
            row.InvoiceId = invoice.Id;
            _context.InvoiceRows.Add(row);
        }

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
        if (invoice.Status != InvoiceStatus.Created) return null;

        _mapper.Map(dto, invoice);
        await _context.SaveChangesAsync();

        return _mapper.Map<InvoiceResponseDto>(invoice);
    }

    public async Task<bool> ChangeStatusAsync(int id, InvoiceStatus status)
    {
        var invoice = await _context.Invoices.FindAsync(id);
        if (invoice is null) return false;

        invoice.Status = status;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteHardAsync(int id)
    {
        var invoice = await _context.Invoices.FindAsync(id);
        if (invoice is null) return false;
        if (invoice.Status != InvoiceStatus.Created) return false;

        _context.Invoices.Remove(invoice);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteSoftAsync(int id)
    {
        var invoice = await _context.Invoices.FindAsync(id);
        if (invoice is null) return false;

        invoice.DeletedAt = DateTimeOffset.UtcNow;
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
}
