using AutoMapper;
using InvoiceManager.DTOs.CustomerDto;
using InvoiceManager.DTOs.InvoiceDto;
using InvoiceManager.DTOs.InvoiceRowDto;
using InvoiceManager.Models;

namespace InvoiceManager.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Customer
        CreateMap<Customer, CustomerResponseDto>();

        CreateMap<CreateCustomerDto, Customer>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.DeletedAt, o => o.Ignore());

        CreateMap<UpdateCustomerDto, Customer>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.DeletedAt, o => o.Ignore());

        // Invoice
        CreateMap<CreateInvoiceDto, Invoice>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Status, o => o.Ignore())
            .ForMember(d => d.Rows, o => o.Ignore()) //add
            .ForMember(d => d.TotalSum, o => o.Ignore()) //add
            .ForMember(d => d.CreatedAt, o => o.Ignore()) //add
            .ForMember(d => d.UpdatedAt, o => o.Ignore()); //add

        CreateMap<CreateInvoiceRowDto, InvoiceRow>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Sum, o => o.Ignore()); //add

        CreateMap<InvoiceRow, InvoiceRowResponseDto>(); //change
        CreateMap<Invoice, InvoiceResponseDto>(); //change
    }
}
