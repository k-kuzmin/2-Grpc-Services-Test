using AutoMapper;
using Database_Service;
using Entities;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace Services;

public class CurrencyGrpcService : CurrencyService.CurrencyServiceBase
{
    private readonly ILogger<CurrencyGrpcService> _logger;
    private readonly IRepository<Currency> _currencyRepository;
    private readonly IMapper _mapper;

    public CurrencyGrpcService(
        ILogger<CurrencyGrpcService> logger,
        IRepository<Currency> repository,
        IMapper mapper)
    {
        _logger = logger;
        _currencyRepository = repository;
        _mapper = mapper;
    }

    public override async Task<CurrencyReply> UpdateCurrency(UpdateCurrencyRequest request, ServerCallContext context)
    {
        var currency = await _currencyRepository.GetAll().FirstOrDefaultAsync(x => x.Code == request.Code);
        if (currency == null)
        {
            currency = await _currencyRepository.Create(_mapper.Map<Currency>(request), context.CancellationToken);
        }
        else
        {
            currency = _mapper.Map<Currency>(request);
            await _currencyRepository.Update(currency, context.CancellationToken);
        }

        await _currencyRepository.Save(context.CancellationToken);

        return _mapper.Map<CurrencyReply>(currency);
    }
}
