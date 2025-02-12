using Controllers;
using Domain;
using DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using Services;

namespace Unit_Tests;

public class UserControllerTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<SignInManager<User>> _signInManagerMock;
    private readonly Mock<AuthService> _authServiceMock;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _userManagerMock = new Mock<UserManager<User>>(
            new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null
        );

        _signInManagerMock = new Mock<SignInManager<User>>(
            _userManagerMock.Object, new Mock<IHttpContextAccessor>().Object, new Mock<IUserClaimsPrincipalFactory<User>>().Object, null, null, null, null
        );

        _authServiceMock = new Mock<AuthService>(
            new Mock<IOptions<JwtSettings>>().Object
        );
        _controller = new UserController(_userManagerMock.Object, _signInManagerMock.Object, _authServiceMock.Object);
    }

    [Fact]
    public async Task Register_ShouldReturnOk_WhenUserCreatedSuccessfully()
    {
        var request = new RegisterRequest("testuser", "test@example.com","Password123!");
        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), request.Password))
            .ReturnsAsync(IdentityResult.Success);

        var result = await _controller.Register(request);
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("User registered successfully", okResult.Value);
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenUserCreationFails()
    {
        var request = new RegisterRequest("testuser", "test@example.com", "Password123!");
        var identityErrors = new List<IdentityError> { new IdentityError { Description = "Error" } };
        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), request.Password))
            .ReturnsAsync(IdentityResult.Failed(identityErrors.ToArray()));

        var result = await _controller.Register(request);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(identityErrors, badRequestResult.Value);
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WithToken_WhenCredentialsAreValid()
    {
        var request = new LoginRequest("testuser", "Password123!");
        var user = new User { UserName = request.Username };
        _userManagerMock.Setup(x => x.FindByNameAsync(request.Username))
            .ReturnsAsync(user);
        _signInManagerMock.Setup(x => x.PasswordSignInAsync(request.Username, request.Password, false, false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
        _authServiceMock.Setup(x => x.GenerateJwtToken(user)).Returns("test_token");

        var result = await _controller.Login(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<LoginResponse>(okResult.Value);

        Assert.Equal("test_token", response.Token);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenUserNotFound()
    {
        var request = new LoginRequest("testuser", "Password123!");
        _userManagerMock.Setup(x => x.FindByNameAsync(request.Username))
            .ReturnsAsync((User)null);

        var result = await _controller.Login(request);
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("Invalid username or password", unauthorizedResult.Value);
    }

    [Fact]
    public async Task Logout_ShouldReturnOk()
    {
        _signInManagerMock.Setup(x => x.SignOutAsync()).Returns(Task.CompletedTask);

        var result = await _controller.Logout();
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Logged out", okResult.Value);
    }
}