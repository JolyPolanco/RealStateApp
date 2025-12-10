using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Unit.Tests.Features.Agents.Queries
{
    using AutoMapper;
    using Moq;
    using RealStateApp.Core.Application.Dtos.User;
    using RealStateApp.Core.Application.Exceptions;
    using RealStateApp.Core.Application.Features.Agents.Queries.GetById;
    using RealStateApp.Core.Application.Interfaces;
    using RealStateApp.Core.Domain.Interfaces;
    using RealStateApp.Infraestructure.Identity.Entities;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class GetAgentByIdQueryHandlerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IPropertyRepository> _propertyRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetAgentByIdQueryHandler _handler;

        public GetAgentByIdQueryHandlerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _propertyRepositoryMock = new Mock<IPropertyRepository>();
            _mapperMock = new Mock<IMapper>();

            _handler = new GetAgentByIdQueryHandler(
                _userServiceMock.Object,
                _propertyRepositoryMock.Object,
                _mapperMock.Object
            );
        }
        [Fact]
        public async Task Handle_ShouldReturnAgentDto_WhenAgentExists()
        {
            // Arrange
            var agentId = Guid.NewGuid().ToString();

            var dto = new UserDto
            {
                Id = agentId,
                UserName = "UserName",
                Dni = "2023450933",
                FirstName = "John",
                LastName = "Doe",
                Email = "john@domain.com",
                Role = "AGENT"
            };

            var mappedDto = new AgentDto
            {
                Id = agentId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@domain.com"
            };

            _userServiceMock
                .Setup(s => s.GetById(agentId))
                .ReturnsAsync(dto);

            _propertyRepositoryMock
                .Setup(r => r.GetAgentPropertiesCount(agentId))
                .ReturnsAsync(5);

            // El handler usa UserDto, no AppUser → se ajusta el mock
            _mapperMock
                .Setup(m => m.Map<AgentDto>(It.IsAny<UserDto>()))
                .Returns(mappedDto);

            var query = new GetAgentByIdQuery { Id = agentId };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(agentId, result.Id);
            Assert.Equal(5, result.PropertiesCount);

            _userServiceMock.Verify(s => s.GetById(agentId), Times.Once);
            _propertyRepositoryMock.Verify(r => r.GetAgentPropertiesCount(agentId), Times.Once);

            _mapperMock.Verify(
                m => m.Map<AgentDto>(It.IsAny<UserDto>()),
                Times.Once
            );
        }


        [Fact]
        public async Task Handle_ShouldThrowApiException_WhenAgentDoesNotExist()
        {
            // Arrange
            var agentId = Guid.NewGuid().ToString();

            // El servicio debe devolver null para simular que el agente no existe
            _userServiceMock
                .Setup(s => s.GetById(agentId))
                .ReturnsAsync((UserDto)null);

            var query = new GetAgentByIdQuery { Id = agentId };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApiException>(() =>
                _handler.Handle(query, CancellationToken.None)
            );

            Assert.Equal("Agent not found with Id", ex.Message);
            _userServiceMock.Verify(s => s.GetById(agentId), Times.Once);
        }
    }

    }
