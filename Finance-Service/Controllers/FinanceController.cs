using Database_Service;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance_Service.Controllers
{
    [ApiController]
    [Route("api/finance")]
    public class FinanceController(
        CurrencyService.CurrencyServiceClient currencyService) : ControllerBase
    {
        [HttpGet("getAllCurrencies")]
        [Authorize]
        public async Task<IActionResult> GetAllCurrencies(CancellationToken cancellationToken)
        {
            var response = await currencyService.GetAllAsync(new Empty(), 
                GetAuthorizationHeader(), 
                null,
                cancellationToken: cancellationToken);

            return Ok(response);
        }

        [HttpGet("getFavoriteCurrencies")]
        [Authorize]
        public async Task<IActionResult> GetFavoriteCurrencies(CancellationToken cancellationToken)
        {
            var username = User.Identity?.Name;
            var response = await currencyService.GetFavoriteCurrenciesAsync(new UserInfoRequest
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
            var response = await currencyService.AddFavoriteCurrencyAsync(new AddFavoriteCurrencyRequest
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
