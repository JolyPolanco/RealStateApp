using FluentAssertions;
using MediatR;
using Moq;
using RealStateApp.Core.Application.Dtos.User;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Application.Features.Agents.Commands.DeleteAgent;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using PropertyEntity = RealStateApp.Core.Domain.Entities.Property;

namespace RealStateApp.Unit.Tests.Features.Agents.Commands
{
    public class DeleteAgentCommandHandlerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IPropertyRepository> _propertyRepositoryMock;
        private readonly DeleteAgentCommandHandler _handler;

        public DeleteAgentCommandHandlerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _propertyRepositoryMock = new Mock<IPropertyRepository>();
            _handler = new DeleteAgentCommandHandler(
                _userServiceMock.Object,
                _propertyRepositoryMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldDeleteAgentAndProperties_WhenAgentExists()
        {
            // Arrange
            var agentId = Guid.NewGuid().ToString();

            var agentDto = new UserDto
            {
                Id = agentId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                UserName = "johndoe",
                Dni = "12345678",
                Role = "AGENT"
            };

            var deleteResponse = new UserResponseDto
            {
                HasError = false,
                Errors = new List<string>()
            };

            _userServiceMock
                .Setup(s => s.GetById(agentId))
                .ReturnsAsync(agentDto);

            _propertyRepositoryMock
                .Setup(r => r.DeleteAgentProperties(agentId))
                .Returns(Task.CompletedTask);

            _userServiceMock
                .Setup(s => s.DeleteAsync(agentId))
                .ReturnsAsync(deleteResponse);

            var command = new DeleteAgentCommand { Id = agentId };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(MediatR.Unit.Value);
            
            _userServiceMock.Verify(s => s.GetById(agentId), Times.Once);
            _propertyRepositoryMock.Verify(r => r.DeleteAgentProperties(agentId), Times.Once);
            _userServiceMock.Verify(s => s.DeleteAsync(agentId), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowApiException_WhenAgentDoesNotExist()
        {
            // Arrange
            var agentId = Guid.NewGuid().ToString();

            _userServiceMock
                .Setup(s => s.GetById(agentId))
                .ReturnsAsync((UserDto?)null);

            var command = new DeleteAgentCommand { Id = agentId };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApiException>(() =>
                _handler.Handle(command, CancellationToken.None)
            );

            ex.Message.Should().Be("Agent not found");
            ((int)ex.StatusCode).Should().Be((int)HttpStatusCode.NotFound);
            
            _userServiceMock.Verify(s => s.GetById(agentId), Times.Once);
            _propertyRepositoryMock.Verify(r => r.DeleteAgentProperties(It.IsAny<string>()), Times.Never);
            _userServiceMock.Verify(s => s.DeleteAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldDeleteAgentWithoutProperties_WhenAgentHasNoProperties()
        {
            // Arrange
            var agentId = Guid.NewGuid().ToString();

            var agentDto = new UserDto
            {
                Id = agentId,
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@example.com",
                UserName = "janesmith",
                Dni = "87654321",
                Role = "AGENT"
            };

            var deleteResponse = new UserResponseDto
            {
                HasError = false,
                Errors = new List<string>()
            };

            _userServiceMock
                .Setup(s => s.GetById(agentId))
                .ReturnsAsync(agentDto);

            _propertyRepositoryMock
                .Setup(r => r.DeleteAgentProperties(agentId))
                .Returns(Task.CompletedTask);

            _userServiceMock
                .Setup(s => s.DeleteAsync(agentId))
                .ReturnsAsync(deleteResponse);

            var command = new DeleteAgentCommand { Id = agentId };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(MediatR.Unit.Value);
            
            _userServiceMock.Verify(s => s.GetById(agentId), Times.Once);
            _propertyRepositoryMock.Verify(r => r.DeleteAgentProperties(agentId), Times.Once);
            _userServiceMock.Verify(s => s.DeleteAsync(agentId), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowApiException_WhenDeleteAgentFails()
        {
            // Arrange
            var agentId = Guid.NewGuid().ToString();

            var agentDto = new UserDto
            {
                Id = agentId,
                FirstName = "Bob",
                LastName = "Johnson",
                Email = "bob@example.com",
                UserName = "bobjohnson",
                Dni = "11223344",
                Role = "AGENT"
            };

            var deleteResponse = new UserResponseDto
            {
                HasError = true,
                Errors = new List<string> { "Error deleting user" }
            };

            _userServiceMock
                .Setup(s => s.GetById(agentId))
                .ReturnsAsync(agentDto);

            _propertyRepositoryMock
                .Setup(r => r.DeleteAgentProperties(agentId))
                .Returns(Task.CompletedTask);

            _userServiceMock
                .Setup(s => s.DeleteAsync(agentId))
                .ReturnsAsync(deleteResponse);

            var command = new DeleteAgentCommand { Id = agentId };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApiException>(() =>
                _handler.Handle(command, CancellationToken.None)
            );

            ex.Message.Should().Be("Error deleting agent");
            ((int)ex.StatusCode).Should().Be((int)HttpStatusCode.InternalServerError);
            
            _userServiceMock.Verify(s => s.GetById(agentId), Times.Once);
            _propertyRepositoryMock.Verify(r => r.DeleteAgentProperties(agentId), Times.Once);
            _userServiceMock.Verify(s => s.DeleteAsync(agentId), Times.Once);
        }
    }
}
