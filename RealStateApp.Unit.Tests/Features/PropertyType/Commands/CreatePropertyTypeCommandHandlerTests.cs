using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Application.Features.Improvement.Commands.CreateImprovement;
using RealStateApp.Core.Application.Features.PropertyType.Commands.Create;
using RealStateApp.Core.Domain.Interfaces;
using RealStateApp.Infraestructure.Persistence.Contexts;
using RealStateApp.Infraestructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Unit.Tests.Features.PropertyType.Commands
{
    public class CreatePropertyTypeCommandHandlerTests
    {
        private readonly DbContextOptions<RealStateContext> _dbContextOptions;

        public CreatePropertyTypeCommandHandlerTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<RealStateContext>()
                .UseInMemoryDatabase(databaseName: $"RealStateAppTest_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task Handle_ShouldReturnPropertyTypeId_WhenCreationIsSuccessful()
        {
            using var context = new RealStateContext(_dbContextOptions);
            var repository = new PropertyTypeRepository(context);

            var handler = new CreatePropertyTypeCommandHandler(repository);

            var command = new CreatePropertyTypeCommand()
            {
                Name = "Property type test",
                Description = "Description test",
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeGreaterThan(0);

            var createdEntity = await context.PropertyTypes.FindAsync(result);

            createdEntity.Should().NotBeNull();
            createdEntity!.Name.Should().Be(command.Name);
            createdEntity.Description.Should().Be(command.Description);
        }


        [Fact]
        public async Task Handle_ShouldReturnZero_WhenRepositoryReturnsNull()
        {
            // Arrange
            Mock<IPropertyTypeRepository> _mockRepository = new();
            CreatePropertyTypeCommandHandler _handler = new(_mockRepository.Object);

            var command = new CreatePropertyTypeCommand()
            {
                Name = "Propertype test",
                Description = "Description test",
            };


            _mockRepository
                   .Setup(r => r.AddAsync(It.IsAny<Core.Domain.Entities.PropertyType>()))
                   .ReturnsAsync((Core.Domain.Entities.PropertyType?)null);

            // Act && Assert
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ApiException>().WithMessage("Error creating Property Type");
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Core.Domain.Entities.PropertyType>()), Times.Once);
        }
    }
}
