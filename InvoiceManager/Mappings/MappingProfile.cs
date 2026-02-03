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
            .ForMember(d => d.Rows, o => o.Ignore());

        CreateMap<CreateInvoiceRowDto, InvoiceRow>()
            .ForMember(d => d.Id, o => o.Ignore());

        CreateMap<InvoiceRow, InvoiceRowResponseDto>()
            .ForMember(d => d.Sum, o => o.MapFrom(s => Decimal.Round(s.Quantity * s.Rate, 2, MidpointRounding.AwayFromZero)));

        CreateMap<Invoice, InvoiceResponseDto>()
            .ForMember(d => d.TotalSum, o => o.MapFrom(s =>
                s.Rows.Sum(r => Decimal.Round(r.Quantity * r.Rate, 2, MidpointRounding.AwayFromZero))
            ))
            .ForMember(d => d.Rows, o => o.MapFrom(s => s.Rows));
    }
}
