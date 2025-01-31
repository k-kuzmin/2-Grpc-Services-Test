using AutoMapper;
using Database_Service;
using Domain.Entities;
using Grpc.Core;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Services;

public class CurrencyGrpcService : CurrencyService.CurrencyServiceBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CurrencyGrpcService> _logger;
    private readonly IRepository<Currency> _currencyRepository;
    private readonly IMapper _mapper;

    public CurrencyGrpcService(
        IUnitOfWork unitOfWork,
        ILogger<CurrencyGrpcService> logger,
        IRepository<Currency> repository,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _currencyRepository = repository;
        _mapper = mapper;
    }

    public override async Task<CurrencyReply> UpdateCurrency(UpdateCurrencyRequest request, ServerCallContext context)
    {
        var currency = await _currencyRepository.GetAll().FirstOrDefaultAsync(x => x.Code == request.Code, context.CancellationToken);
        if (currency == null)
        {
            currency = await _currencyRepository.Create(_mapper.Map<Currency>(request), context.CancellationToken);
        }
        else
        {
            currency = _mapper.Map(request, currency);
            await _currencyRepository.Update(currency, context.CancellationToken);
        }

        await _unitOfWork.Commit(context.CancellationToken);

        return _mapper.Map<CurrencyReply>(currency);
    }
}
