using MediatR;
using Moq;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Application.Features.Agents.Commands.ChangeStatus;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Infraestructure.Identity.Entities;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class ChangeAgentStatusCommandHandlerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly ChangeAgentStatusCommandHandler _handler;

    public ChangeAgentStatusCommandHandlerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _handler = new ChangeAgentStatusCommandHandler(_userServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnit_WhenStatusIsUpdated()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        var command = new ChangeAgentStatusCommand
        {
            Id = agentId.ToString(),
            Status = true
        };

        _userServiceMock
            .Setup(s => s.SetStatus(agentId.ToString(), true))
            .ReturnsAsync(true);  // ← Escenario positivo

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(Unit.Value, result);
        _userServiceMock.Verify(s => s.SetStatus(agentId.ToString(), true), Times.Once);
    }


    [Fact]
    public async Task Handle_ShouldThrowApiException_WhenSetStatusFails()
    {
        var agentId = Guid.NewGuid();

        // Arrange
        var command = new ChangeAgentStatusCommand { Id = agentId.ToString(), Status = false };
        _userServiceMock
            .Setup(s => s.SetStatus(command.Id, command.Status))
            .ReturnsAsync(false);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ApiException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Entity Not found", ex.Message);
        _userServiceMock.Verify(s => s.SetStatus(command.Id, command.Status), Times.Once);
    }
}
