using Moq;
using RealStateApp.Core.Application.Dtos.Login;
using RealStateApp.Core.Application.Features.Login.Commands;
using RealStateApp.Core.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Unit.Tests.Features.Login.Commands
{
    public class LoginCommandHandlerTests
    {



        [Fact]
        public async Task Handle_ShouldReturnLoginResponse_WhenCredentialsAreValid()
        {
            // Arrange
            var mockService = new Mock<IAccountServiceForWebApi>();

            mockService
                .Setup(m => m.AuthenticateAsync(
                    It.Is<LoginDto>(dto =>
                        dto.Username == "test@mail.com" &&
                        dto.Password == "1234"
                    )
                ))
                .ReturnsAsync(new LoginResponseForApi
                {
                    Name = "Test",
                    LastName = "User",
                    HasError = false,
                    AccessToken = "abc123",
                    Errors = new List<string>()
                });

            var handler = new LoginCommandHandler(mockService.Object);

            var command = new LoginCommand
            {
                Username = "test@mail.com",
                Password = "1234"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.HasError);
            Assert.Equal("abc123", result.AccessToken);
            Assert.Equal("Test", result.Name);
            Assert.Equal("User", result.LastName);

            mockService.Verify(m =>
                m.AuthenticateAsync(
                    It.Is<LoginDto>(dto =>
                        dto.Username == "test@mail.com" &&
                        dto.Password == "1234"
                    )
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenCredentialsAreInvalid()
        {
            // Arrange
            var mockService = new Mock<IAccountServiceForWebApi>();

            mockService
                .Setup(m => m.AuthenticateAsync(It.IsAny<LoginDto>()))
                .ReturnsAsync(new LoginResponseForApi
                {
                    Name = "",
                    LastName = "",
                    HasError = true,
                    AccessToken = null,
                    Errors = new List<string> { "Invalid credentials" }
                });

            var handler = new LoginCommandHandler(mockService.Object);

            var command = new LoginCommand
            {
                Username = "wrong@mail.com",
                Password = "badpass"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.HasError);
            Assert.Null(result.AccessToken);
            Assert.Contains("Invalid credentials", result.Errors!);
        }

    }
}
