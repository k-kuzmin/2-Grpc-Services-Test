using System.Security.Claims;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Finance_Service.Controllers;
using Database_Service;

namespace Unit_Tests;

public class FinanceControllerTests
{
    private readonly Mock<CurrencyService.CurrencyServiceClient> _mockCurrencyService;
    private readonly FinanceController _controller;

    public FinanceControllerTests()
    {
        _mockCurrencyService = new Mock<CurrencyService.CurrencyServiceClient>();
        _controller = new FinanceController(_mockCurrencyService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public async Task GetAllCurrencies_ReturnsOkResult()
    {
        var expectedResponse = new CurrencyListReply();

        _mockCurrencyService
            .Setup(service => service.GetAllAsync(It.IsAny<Empty>(), It.IsAny<Metadata>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _controller.GetAllCurrencies(CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedResponse, okResult.Value);
    }

    [Fact]
    public async Task GetFavoriteCurrencies_ReturnsOkResult()
    {
        var expectedResponse = new CurrencyListReply();
        var user = new ClaimsPrincipal(new ClaimsIdentity(new [] { new Claim(ClaimTypes.Name, "testuser") }));
        _controller.ControllerContext.HttpContext.User = user;

        _mockCurrencyService
            .Setup(service => service.GetFavoriteCurrenciesAsync(It.IsAny<UserInfoRequest>(), It.IsAny<Metadata>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _controller.GetFavoriteCurrencies(CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedResponse, okResult.Value);
    }

    [Fact]
    public async Task AddFavoriteCurrencies_ReturnsOkResult()
    {
        var expectedResponse = new ResultReply();
        var user = new ClaimsPrincipal(new ClaimsIdentity(new [] { new Claim(ClaimTypes.Name, "testuser") }));
        _controller.ControllerContext.HttpContext.User = user;

        _mockCurrencyService
            .Setup(service => service.AddFavoriteCurrencyAsync(It.IsAny<AddFavoriteCurrencyRequest>(), It.IsAny<Metadata>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _controller.AddFavoriteCurrencies("USD", CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedResponse, okResult.Value);
    }
}
