using AutoMapper;
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
        var customer = await _context.Customers.FindAsync(id);
        return customer is null ? null : _mapper.Map<CustomerResponseDto>(customer);
    }

    public async Task<CustomerResponseDto?> UpdateAsync(int id, UpdateCustomerDto dto)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer is null) return null;

        _mapper.Map(dto, customer);
        await _context.SaveChangesAsync();

        return _mapper.Map<CustomerResponseDto>(customer);
    }

    public async Task<bool> DeleteSoftAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer is null) return false;

        customer.DeletedAt = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteHardAsync(int id)
    {
        var hasSentInvoices = await _context.Invoices
            .AnyAsync(i => i.CustomerId == id && i.Status != InvoiceStatus.Created);

        if (hasSentInvoices) return false;

        var customer = await _context.Customers.FindAsync(id);
        if (customer is null) return false;

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();

        return true;
    }
}

