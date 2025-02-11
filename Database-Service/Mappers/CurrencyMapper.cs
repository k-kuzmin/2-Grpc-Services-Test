using AutoMapper;
using Database_Service;
using Domain.Entities;

namespace Mappers;

public class CurrencyMapper : Profile
{
    public CurrencyMapper()
    {
        CreateMap<Currency, CurrencyReply>()
            .ForMember(d => d.Rate, opt => opt.MapFrom(s => (double)s.Rate));
        CreateMap<UpdateCurrencyRequest, Currency>()
            .ForMember(d => d.Rate, opt => opt.MapFrom(s => (decimal)s.Rate));
    }
}
