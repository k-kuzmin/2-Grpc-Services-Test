using AutoMapper;
using Database_Service;
using Entities;

namespace Mappers
{
    public class CurrencyMapper : Profile
    {
        public CurrencyMapper()
        {
            CreateMap<Currency, CurrencyReply>();
            CreateMap<UpdateCurrencyRequest, Currency>();
        }
    }
}
