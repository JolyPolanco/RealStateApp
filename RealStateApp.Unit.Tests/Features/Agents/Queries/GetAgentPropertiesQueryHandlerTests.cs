using AutoMapper;
using Moq;
using RealStateApp.Core.Application.Dtos.Properties;
using RealStateApp.Core.Application.Dtos.User;
using RealStateApp.Core.Application.Features.Agents.Queries.GetAgentProperties;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using PropertyEntity = RealStateApp.Core.Domain.Entities.Property;

namespace RealStateApp.Unit.Tests.Features.Agents.Queries
{
    public class GetAgentPropertiesQueryHandlerTests
    {
        private readonly Mock<IPropertyRepository> _propertyRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly GetAgentPropertiesQueryHandler _handler;

        public GetAgentPropertiesQueryHandlerTests()
        {
            _propertyRepositoryMock = new Mock<IPropertyRepository>();
            _mapperMock = new Mock<IMapper>();
            _userServiceMock = new Mock<IUserService>();

            _handler = new GetAgentPropertiesQueryHandler(
                _propertyRepositoryMock.Object,
                _mapperMock.Object,
                _userServiceMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnPropertiesList_WhenAgentHasProperties()
        {
            // Arrange
            var agentId = "agent-123";
            
            var properties = new List<PropertyEntity>
            {
                new PropertyEntity
                {
                    Id = 1,
                    Code = "PROP01",
                    AgentId = agentId,
                    Price = 150000,
                    SizeInMeters = 120,
                    Bedrooms = 3,
                    Bathrooms = 2,
                    Description = "Beautiful house"
                },
                new PropertyEntity
                {
                    Id = 2,
                    Code = "PROP02",
                    AgentId = agentId,
                    Price = 200000,
                    SizeInMeters = 150,
                    Bedrooms = 4,
                    Bathrooms = 3,
                    Description = "Spacious apartment"
                }
            };

            var propertyDtos = new List<PropertyApiDto>
            {
                new PropertyApiDto
                {
                    Id = 1,
                    Code = "PROP01",
                    Price = 150000,
                    SizeInMeters = 120,
                    Bedrooms = 3,
                    Bathrooms = 2,
                    Description = "Beautiful house",
                    PropertyType = "House",
                    SaleType = "Sale",
                    AgentName = "",
                    AgentId = agentId
                },
                new PropertyApiDto
                {
                    Id = 2,
                    Code = "PROP02",
                    Price = 200000,
                    SizeInMeters = 150,
                    Bedrooms = 4,
                    Bathrooms = 3,
                    Description = "Spacious apartment",
                    PropertyType = "Apartment",
                    SaleType = "Sale",
                    AgentName = "",
                    AgentId = agentId
                }
            };

            var agentUser = new UserDto
            {
                Id = agentId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                UserName = "johndoe",
                Dni = "12345678",
                Role = "Agent"
            };

            _propertyRepositoryMock
                .Setup(r => r.GetListByAgentId(agentId))
                .ReturnsAsync(properties);

            _mapperMock
                .Setup(m => m.Map<PropertyApiDto>(It.IsAny<PropertyEntity>()))
                .Returns((PropertyEntity p) => propertyDtos.First(dto => dto.Id == p.Id));

            _userServiceMock
                .Setup(s => s.GetById(agentId))
                .ReturnsAsync(agentUser);

            var query = new GetAgentPropertiesQuery { AgentId = agentId };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, dto => Assert.Equal("John Doe", dto.AgentName));

            _propertyRepositoryMock.Verify(r => r.GetListByAgentId(agentId), Times.Once);
            _userServiceMock.Verify(s => s.GetById(agentId), Times.Exactly(2)); // Una vez por cada propiedad
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenAgentHasNoProperties()
        {
            // Arrange
            var agentId = "agent-456";
            var emptyList = new List<PropertyEntity>();

            _propertyRepositoryMock
                .Setup(r => r.GetListByAgentId(agentId))
                .ReturnsAsync(emptyList);

            var query = new GetAgentPropertiesQuery { AgentId = agentId };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);

            _propertyRepositoryMock.Verify(r => r.GetListByAgentId(agentId), Times.Once);
            _userServiceMock.Verify(s => s.GetById(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldSetAgentNameToUnknown_WhenUserServiceReturnsNull()
        {
            // Arrange
            var agentId = "agent-789";
            
            var properties = new List<PropertyEntity>
            {
                new PropertyEntity
                {
                    Id = 1,
                    Code = "PROP03",
                    AgentId = agentId,
                    Price = 100000,
                    SizeInMeters = 80,
                    Bedrooms = 2,
                    Bathrooms = 1,
                    Description = "Cozy studio"
                }
            };

            var propertyDto = new PropertyApiDto
            {
                Id = 1,
                Code = "PROP03",
                Price = 100000,
                SizeInMeters = 80,
                Bedrooms = 2,
                Bathrooms = 1,
                Description = "Cozy studio",
                PropertyType = "Studio",
                SaleType = "Rent",
                AgentName = "",
                AgentId = agentId
            };

            _propertyRepositoryMock
                .Setup(r => r.GetListByAgentId(agentId))
                .ReturnsAsync(properties);

            _mapperMock
                .Setup(m => m.Map<PropertyApiDto>(It.IsAny<PropertyEntity>()))
                .Returns(propertyDto);

            _userServiceMock
                .Setup(s => s.GetById(agentId))
                .ReturnsAsync((UserDto)null);

            var query = new GetAgentPropertiesQuery { AgentId = agentId };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Unknown", result[0].AgentName);

            _propertyRepositoryMock.Verify(r => r.GetListByAgentId(agentId), Times.Once);
            _userServiceMock.Verify(s => s.GetById(agentId), Times.Once);
        }
    }
}
