using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RealStateApp.Core.Application.Dtos.Properties;
using RealStateApp.Core.Application.Features.Agents.Queries.GetAgentProperties;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Domain.Entities;
using RealStateApp.Core.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RealStateApp.Unit.Tests.Features.Agents.Commands
{
    public class GetAgentDetailsQueryHandlerTest
    {
        private readonly Mock<IPropertyRepository> _mockPropertyRepo;
        private readonly Mock<IUserService> _mockUserService;
        private readonly IMapper _mapper;

        public GetAgentDetailsQueryHandlerTest()
        {
            _mockPropertyRepo = new Mock<IPropertyRepository>();
            _mockUserService = new Mock<IUserService>();
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RealStateApp.Core.Domain.Entities.Property, PropertyApiDto>();
            },loggerFactory);
            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task Handle_ShouldReturnPropertiesWithAgentName_WhenPropertiesExist()
        {
            // Arrange
            var agentId = "agent-123";
            var properties = new List<RealStateApp.Core.Domain.Entities.Property>
            {
                new RealStateApp.Core.Domain.Entities.Property { Id = 1, AgentId = agentId, Code = "P1", Description = "Prop1" },
                new RealStateApp.Core.Domain.Entities.Property { Id = 2, AgentId = agentId, Code = "P2", Description = "Prop2" }
            };

            _mockPropertyRepo
                .Setup(r => r.GetListByAgentId(agentId))
                .ReturnsAsync(properties);

            _mockUserService
                .Setup(s => s.GetById(agentId))
                .ReturnsAsync(new Core.Application.Dtos.User.UserDto
                {
                    Id = agentId,
                    FirstName = "John",
                    LastName = "Doe",
                    Email="test@gmail.com",
                     Dni="12345678903",
                     Role="AGENT",
                     UserName="AGENTUSER"
                });

            var handler = new GetAgentPropertiesQueryHandler(
                _mockPropertyRepo.Object,
                _mapper,
                _mockUserService.Object
            );

            var query = new GetAgentPropertiesQuery { AgentId = agentId };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result.All(p => p.AgentName == "John Doe").Should().BeTrue();
            result.Select(p => p.Code).Should().Contain(new[] { "P1", "P2" });
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoPropertiesExist()
        {
            // Arrange
            var agentId = "agent-456";

            _mockPropertyRepo
                .Setup(r => r.GetListByAgentId(agentId))
                .ReturnsAsync(new List<RealStateApp.Core.Domain.Entities.Property>());

            var handler = new GetAgentPropertiesQueryHandler(
                _mockPropertyRepo.Object,
                _mapper,
                _mockUserService.Object
            );

            var query = new GetAgentPropertiesQuery { AgentId = agentId };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
