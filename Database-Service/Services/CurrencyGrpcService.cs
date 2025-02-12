using AutoMapper;
using AutoMapper.QueryableExtensions;
using Database_Service;
using Domain.Entities;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Services;

public class CurrencyGrpcService : CurrencyService.CurrencyServiceBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CurrencyGrpcService> _logger;
    private readonly IRepository<Currency, Guid> _currencyRepository;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;

    public CurrencyGrpcService(
        IUnitOfWork unitOfWork,
        ILogger<CurrencyGrpcService> logger,
        IRepository<Currency, Guid> currencyRepository,
        IMapper mapper,
        UserManager<User> userManager)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _currencyRepository = currencyRepository;
        _mapper = mapper;
        _userManager = userManager;
    }

    [Authorize]
    public override async Task<CurrencyListReply> GetAll(Empty request, ServerCallContext context)
    {
        try
        {
            var allCurrencies = await _currencyRepository.GetAll()
                .ProjectTo<CurrencyReply>(_mapper.ConfigurationProvider)
                .ToListAsync(context.CancellationToken);

            var result = new CurrencyListReply();
            result.Items.AddRange(allCurrencies);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, ex.Message);
            throw new RpcException(new Status(StatusCode.Aborted, ex.Message));
        }
    }

    public override async Task<ResultReply> UpdateCurrency(UpdateCurrencyRequest request, ServerCallContext context)
    {
        try
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

            return new ResultReply()
            {
                Success = true,
                Message = string.Empty,
            };
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, ex.Message);
            throw new RpcException(new Status(StatusCode.Aborted, ex.Message));
        }
    }

    [Authorize]
    public override async Task<CurrencyListReply> GetFavoriteCurrencies(UserInfoRequest request, ServerCallContext context)
    {
        try
        {
            var favoriteCurrencies = await _currencyRepository.GetAll()
                .Where(x => x.Users.Any(user => user.UserName == request.UserName))
                .ProjectTo<CurrencyReply>(_mapper.ConfigurationProvider)
                .ToListAsync(context.CancellationToken);

            var result = new CurrencyListReply();
            result.Items.AddRange(favoriteCurrencies);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, ex.Message);
            throw new RpcException(new Status(StatusCode.Aborted, ex.Message));
        }
    }

    [Authorize]
    public override async Task<ResultReply> AddFavoriteCurrency(AddFavoriteCurrencyRequest request, ServerCallContext context)
    {
        try
        {
            if (Guid.TryParse(request.CurrencyId, out var currencyId) is false)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, $"Invalid currency id"));
            }

            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            }

            var currency = await _currencyRepository.Get(currencyId, context.CancellationToken);
            if (currency == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Currency not found"));
            }

            user.Favorites.Add(currency);
            await _unitOfWork.Commit(context.CancellationToken);

            return new ResultReply()
            {
                Success = true,
                Message = string.Empty,
            };
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, ex.Message);
            throw new RpcException(new Status(StatusCode.Aborted, ex.Message));
        }
    }
}
