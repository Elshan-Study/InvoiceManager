using AutoMapper;
using InvoiceManager.Common;
using InvoiceManager.Data;
using InvoiceManager.DTOs.CustomerDto;
using InvoiceManager.Models;
using InvoiceManager.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InvoiceManager.Services;

public class CustomerService : ICustomerService
{
    private readonly TaskFlowDbContext _context;
    private readonly IMapper _mapper;

    public CustomerService(TaskFlowDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<CustomerResponseDto> CreateAsync(CreateCustomerDto dto)
    {
        var customer = _mapper.Map<Customer>(dto);
        var now = DateTimeOffset.UtcNow;
        customer.CreatedAt = now; //add
        customer.UpdatedAt = now; //add

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return _mapper.Map<CustomerResponseDto>(customer);
    }

    public async Task<IEnumerable<CustomerResponseDto>> GetAllAsync()
    {
        var customers = await _context.Customers.ToListAsync();
        return _mapper.Map<IEnumerable<CustomerResponseDto>>(customers);
    }

    public async Task<CustomerResponseDto?> GetByIdAsync(int id)
    {
        var customer = await _context.Customers //changed
                        .AsQueryable()
                        .FirstOrDefaultAsync(c => c.Id == id);

        return customer is null ? null : _mapper.Map<CustomerResponseDto>(customer);
    }

    public async Task<CustomerResponseDto?> UpdateAsync(int id, UpdateCustomerDto dto)
    {
        var customer = await _context.Customers //changed
                        .AsQueryable()
                        .FirstOrDefaultAsync(c => c.Id == id);

        if (customer is null) return null;

        _mapper.Map(dto, customer);
        customer.UpdatedAt = DateTimeOffset.UtcNow; //add

        await _context.SaveChangesAsync();

        return _mapper.Map<CustomerResponseDto>(customer);
    }

    public async Task<bool> DeleteSoftAsync(int id)
    {
        var customer = await _context.Customers //changed
                        .AsQueryable()
                        .FirstOrDefaultAsync(c => c.Id == id);
        if (customer is null) return false;

        customer.DeletedAt = DateTimeOffset.UtcNow;
        customer.UpdatedAt = DateTimeOffset.UtcNow; //added

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteHardAsync(int id)
    {
        var hasSentInvoices = await _context.Invoices
            .AnyAsync(i => i.CustomerId == id && i.Status != InvoiceStatus.Created);

        if (hasSentInvoices) return false;

        var customer = await _context.Customers //changed
                        .IgnoreQueryFilters() 
                        .FirstOrDefaultAsync(c => c.Id == id);

        if (customer is null) return false;

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<PagedResult<CustomerResponseDto>> GetPagedAsync(
    int page = 1,
    int pageSize = 10,
    string? nameFilter = null,
    string? sortBy = "Id",
    bool ascending = true)
    {
        var query = _context.Customers
                            .Where(c => c.DeletedAt == null) // только активные
                            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(nameFilter))
            query = query.Where(c => c.Name.Contains(nameFilter));

        query = sortBy?.ToLower() switch
        {
            "name" => ascending ? query.OrderBy(c => c.Name) : query.OrderByDescending(c => c.Name),
            "email" => ascending ? query.OrderBy(c => c.Email) : query.OrderByDescending(c => c.Email),
            _ => ascending ? query.OrderBy(c => c.Id) : query.OrderByDescending(c => c.Id)
        };

        var totalCount = await query.CountAsync();
        var items = await query
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();

        var mapped = _mapper.Map<IEnumerable<CustomerResponseDto>>(items);

        return PagedResult<CustomerResponseDto>.Create(mapped, page, pageSize, totalCount);
    }

}

