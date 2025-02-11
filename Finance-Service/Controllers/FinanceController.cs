using Database_Service;
using Google.Protobuf.WellKnownTypes;
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
            var responce = await _currencyService.GetAllAsync(new Empty(), cancellationToken: cancellationToken);
            
            return Ok(responce);
        }

        [HttpGet("getFavoriteCurrencies")]
        [Authorize]
        public async Task<IActionResult> GetFavoriteCurrencies(CancellationToken cancellationToken)
        {
            var username = User.Identity?.Name;
            var responce = await _currencyService.GetFavoriteCurrenciesAsync(new UserInfoRequest
            {
                UserName = username
            }, cancellationToken: cancellationToken);
            
            return Ok(responce);
        }

        [HttpPost("addFavoriteCurrencies")]
        [Authorize]
        public async Task<IActionResult> AddFavoriteCurrencies(string currencyId, CancellationToken cancellationToken)
        {
            var username = User.Identity?.Name;
            var responce = await _currencyService.AddFavoriteCurrencyAsync(new AddFavoriteCurrencyRequest
            {
                UserName = username,
                CurrencyId = currencyId
            }, cancellationToken: cancellationToken);

            return Ok(responce);
        }
    }
}
