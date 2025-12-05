using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Unit.Tests.Features.Agents.Queries
{
    using Moq;
    using RealStateApp.Core.Application.Dtos.User;
    using RealStateApp.Core.Application.Exceptions;
    using RealStateApp.Core.Application.Features.Agents.Queries.GetAllList;
    using RealStateApp.Core.Application.Interfaces;
    using RealStateApp.Core.Domain.Interfaces;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class GetAllAgentsQueryHandlerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IPropertyRepository> _propertyRepositoryMock;
        private readonly GetAllAgentsListQueryHandler _handler;

        public GetAllAgentsQueryHandlerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _propertyRepositoryMock = new Mock<IPropertyRepository>();

            _handler = new GetAllAgentsListQueryHandler(
                _userServiceMock.Object,
                _propertyRepositoryMock.Object
            );
        }

        // -----------------------------------------------------------
        // ✔️ 1. CASO POSITIVO
        // -----------------------------------------------------------
        [Fact]
        public async Task Handle_ShouldReturnAgentsList_WhenDataIsValid()
        {
            // Arrange
            var propertyCount = new Dictionary<string, int>
        {
            { "1", 3 },
            { "2", 5 }
        };

            var agents = new List<AgentDto>
        {
            new AgentDto { Id = "1", FirstName = "John", LastName = "Doe", PropertiesCount = 3 , Email="Email@gmail.com"},
            new AgentDto { Id = "2", FirstName = "Sarah", LastName = "Smith", PropertiesCount = 5 , Email = "Email@gmail.com"}
        };

            _propertyRepositoryMock
                .Setup(r => r.GetAgentsPropertiesCount())
                .ReturnsAsync(propertyCount);

            _userServiceMock
                .Setup(s => s.GetUsersAgentOnly(propertyCount))
                .ReturnsAsync(agents);

            var query = new GetAllAgentsListQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            _propertyRepositoryMock.Verify(r => r.GetAgentsPropertiesCount(), Times.Once);
            _userServiceMock.Verify(s => s.GetUsersAgentOnly(propertyCount), Times.Once);
        }

        // -----------------------------------------------------------
        // ✔️ 2. CASO NEGATIVO
        // -----------------------------------------------------------
        [Fact]
        public async Task Handle_ShouldThrowApiException_WhenUsersListIsNull()
        {
            // Arrange
            var propertyCount = new Dictionary<string, int>
        {
            { "1", 3 }
        };

            _propertyRepositoryMock
        .Setup(r => r.GetAgentsPropertiesCount())
        .ReturnsAsync(propertyCount);

            _userServiceMock
                .Setup(s => s.GetUsersAgentOnly(propertyCount))
                .Returns(Task.FromResult<IList<AgentDto>>(null));



            var query = new GetAllAgentsListQuery();

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApiException>(() =>
                _handler.Handle(query, CancellationToken.None)
            );

            Assert.Equal("Agents data not available", ex.Message);

            _propertyRepositoryMock.Verify(r => r.GetAgentsPropertiesCount(), Times.Once);
            _userServiceMock.Verify(s => s.GetUsersAgentOnly(propertyCount), Times.Once);
        }
    }

}
