using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Application.Features.Improvement.Commands.DeleteImprovement;
using RealStateApp.Core.Application.Features.PropertyType.Commands.Delete;
using RealStateApp.Infraestructure.Persistence.Contexts;
using RealStateApp.Infraestructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Unit.Tests.Features.PropertyType.Commands
{
    public class DeletePropertyTypeCommandHandlerTests
    {
        private readonly DbContextOptions<RealStateContext> _dbContextOptions;
        public DeletePropertyTypeCommandHandlerTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<RealStateContext>()
              .UseInMemoryDatabase(databaseName: $"RealStateAppTest_{Guid.NewGuid()}")
              .Options;
        }

        [Fact]
        public async Task Handle_ShouldDeletePropertyType_WhenIdExists()
        {
            // Arrange
            using var context = new RealStateContext(_dbContextOptions);
            var existingEntity = new Core.Domain.Entities.PropertyType
            {
                Id = 1,
                Name = "ToDelete",
                Description = "Should be deleted"
            };
            context.PropertyTypes.Add(existingEntity);
            await context.SaveChangesAsync();

            var repository = new PropertyTypeRepository(context);
            var handler = new DeletePropertyTypeCommandHandler(repository);
            var command = new DeletePropertyTypeCommand { Id = existingEntity.Id };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(MediatR.Unit.Value);
            var deleted = await context.Improvements.FindAsync(existingEntity.Id);
            deleted.Should().BeNull();
        }



        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenImprovementNotFound()
        {
            // Arrange
            using var context = new RealStateContext(_dbContextOptions);
            var repository = new PropertyTypeRepository(context);
            var handler = new DeletePropertyTypeCommandHandler(repository);
            var command = new DeletePropertyTypeCommand { Id = 9999 };

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ApiException>()
                .WithMessage("Entity not found with this id");
        }
    }

}
