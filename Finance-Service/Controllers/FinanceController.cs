using Database_Service;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance_Service.Controllers
{
    [ApiController]
    [Route("api/finance")]
    public class FinanceController : ControllerBase
    {
        private readonly CurrencyService.CurrencyServiceClient _currencyService;

        public FinanceController(
            CurrencyService.CurrencyServiceClient currencyService)
        {
            _currencyService = currencyService;
        }

        [HttpGet("getAllCurrencies")]
        [Authorize]
        public async Task<IActionResult> GetAllCurrencies(CancellationToken cancellationToken)
        {
            var response = await _currencyService.GetAllAsync(new Empty(), 
                GetAuthorizationHeader(), 
                cancellationToken: cancellationToken);

            return Ok(response);
        }

        [HttpGet("getFavoriteCurrencies")]
        [Authorize]
        public async Task<IActionResult> GetFavoriteCurrencies(CancellationToken cancellationToken)
        {
            var username = User.Identity?.Name;
            var response = await _currencyService.GetFavoriteCurrenciesAsync(new UserInfoRequest
            {
                UserName = username
            },
            GetAuthorizationHeader(),
            cancellationToken: cancellationToken);
            
            return Ok(response);
        }

        [HttpPost("addFavoriteCurrencies")]
        [Authorize]
        public async Task<IActionResult> AddFavoriteCurrencies(string currencyId, CancellationToken cancellationToken)
        {
            var username = User.Identity?.Name;
            var response = await _currencyService.AddFavoriteCurrencyAsync(new AddFavoriteCurrencyRequest
            {
                UserName = username,
                CurrencyId = currencyId
            }, 
            GetAuthorizationHeader(),
            cancellationToken: cancellationToken);

            return Ok(response);
        }

        private Metadata GetAuthorizationHeader()
        {
            var token = Request.Headers.Authorization.ToString();
            var headers = new Metadata
            {
                { "Authorization", token }
            };
            return headers;
        }
    }
}
