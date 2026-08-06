using Moq;
using FluentAssertions;
using LxzdBxy.WebApi.Application.Common.Interfaces;
using LxzdBxy.WebApi.Application.Features.Auth.Handlers;
using LxzdBxy.WebApi.Application.Features.Commands;
using LxzdBxy.WebApi.Domain.Entities;
using LxzdBxy.WebApi.Application.Features.Responses;
using LxzdBxy.WebApi.Application.Common.Exceptions;
using LxzdBxy.WebApi.Application.Common.Models;

namespace Lxzd.WebApi.Test.Application.UnitTests;

public class LoginCommandHandlerTests
{
    private readonly Mock<IIdentityUserRepository> _userRepoMock;
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepoMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _userRepoMock = new Mock<IIdentityUserRepository>();
        _refreshTokenRepoMock = new Mock<IRefreshTokenRepository>();
        _jwtServiceMock = new Mock<IJwtService>();

        _handler = new LoginCommandHandler(
            _userRepoMock.Object,
            _refreshTokenRepoMock.Object,
            _jwtServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsLoginResponseWithTokensAndSavesRefreshToken()
    {
        var user = new UserClaimsDto("user-123", "test@example.com");
        var command = new LoginCommand("test@example.com", "correct-password");
        var expectedAccessToken = "jwt-access-token";
        var expectedRefreshToken = "secure-refresh-token";

        _userRepoMock
            .Setup(x => x.FindByEmailAsync(command.Email))
            .Returns(() => Task.FromResult<UserClaimsDto?>(user));

        _userRepoMock
            .Setup(x => x.CheckPasswordAsync(user, command.Password))
            .Returns(() => Task.FromResult(true));

        _jwtServiceMock
            .Setup(x => x.GenerateAccessToken(user))
            .Returns(expectedAccessToken);

        _jwtServiceMock
            .Setup(x => x.GenerateRefreshToken())
            .Returns(expectedRefreshToken);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<LoginResponse>();
        result.Value.AccessToken.Should().Be(expectedAccessToken);
        result.Value.RefreshToken.Should().Be(expectedRefreshToken);

        _refreshTokenRepoMock.Verify(
            x => x.Add(It.Is<RefreshToken>(rt =>
                rt.Token == expectedRefreshToken &&
                rt.UserId == user.Id &&
                rt.ExpiresAt > DateTime.UtcNow &&
                rt.CreatedAt <= DateTime.UtcNow &&
                rt.IsRevoked == false)),
            Times.Once);

        _refreshTokenRepoMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsUserNotFoundError()
    {
        // Arrange
        var command = new LoginCommand("unknown@example.com", "any-password");

        _userRepoMock
            .Setup(x => x.FindByEmailAsync(command.Email))
            .Returns(() => Task.FromResult<UserClaimsDto?>(null));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(AuthException.UserNotFound);

        _userRepoMock.Verify(
            x => x.CheckPasswordAsync(It.IsAny<UserClaimsDto>(), It.IsAny<string>()),
            Times.Never);

        _refreshTokenRepoMock.Verify(x => x.Add(It.IsAny<RefreshToken>()), Times.Never);
        _refreshTokenRepoMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_IncorrectPassword_ReturnsIncorrectPasswordError()
    {
        var user = new UserClaimsDto("user-123", "test@example.com");
        var command = new LoginCommand("test@example.com", "wrong-password");

        _userRepoMock
            .Setup(x => x.FindByEmailAsync(command.Email))
            .Returns(() => Task.FromResult<UserClaimsDto?>(user));

        _userRepoMock
            .Setup(x => x.CheckPasswordAsync(user, command.Password))
            .Returns(() => Task.FromResult(false));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(AuthException.IncorrectPassword);

        _jwtServiceMock.Verify(x => x.GenerateAccessToken(It.IsAny<UserClaimsDto>()), Times.Never);
        _jwtServiceMock.Verify(x => x.GenerateRefreshToken(), Times.Never);

        _refreshTokenRepoMock.Verify(x => x.Add(It.IsAny<RefreshToken>()), Times.Never);
        _refreshTokenRepoMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}