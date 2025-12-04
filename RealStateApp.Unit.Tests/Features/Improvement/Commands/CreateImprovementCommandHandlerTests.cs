using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Application.Features.Improvement.Commands.CreateImprovement;
using RealStateApp.Core.Application.Features.Property.Commands.CreateProperty;
using RealStateApp.Core.Application.Features.SaleType.Commands.CreateSaleType;
using RealStateApp.Core.Domain.Interfaces;
using RealStateApp.Infraestructure.Persistence.Contexts;
using RealStateApp.Infraestructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Unit.Tests.Features.Improvement.Commands
{
    public class CreateImprovementCommandHandlerTests
    {

        private readonly DbContextOptions<RealStateContext> _dbContextOptions;

        public CreateImprovementCommandHandlerTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<RealStateContext>()
                .UseInMemoryDatabase(databaseName: $"RealStateAppTest_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]

        public async Task Handle_ShouldReturnImprovementId_WhenCreationIsSuccessful()
        {
            using var context = new RealStateContext(_dbContextOptions);
            var repository = new ImprovementRepository(context);

            CreateImprovementCommandHandler handler = new CreateImprovementCommandHandler(repository);

            var command = new CreateImprovementCommand()
            {
                Name = "Improvement test",
                Description = "Description test",
            };


            //Act
            var result = await handler.Handle(command, CancellationToken.None);


            //Assert
            result.Should().BeGreaterThan(0);
            var createdEntity = await context.SaleTypes.FindAsync(result);
            createdEntity.Should().NotBeNull();
            createdEntity!.Name.Should().Be(command.Name);
            createdEntity.Description.Should().Be(command.Description);
        }

        [Fact]
        public async Task Handle_ShouldReturnZero_WhenRepositoryReturnsNull()
        {
            // Arrange
            Mock<IImprovementRepository> _mockRepository = new();
            CreateImprovementCommandHandler _handler = new(_mockRepository.Object);

            var command = new CreateImprovementCommand()
            {
                Name = "Improvement test",
                Description = "Description test",
            };


            _mockRepository
                   .Setup(r => r.AddAsync(It.IsAny<Core.Domain.Entities.Improvement>()))
                   .ReturnsAsync((Core.Domain.Entities.Improvement?)null);

            // Act && Assert
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ApiException>().WithMessage("Error creating improvement");
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Core.Domain.Entities.Improvement>()), Times.Once);
        }

    }
}
