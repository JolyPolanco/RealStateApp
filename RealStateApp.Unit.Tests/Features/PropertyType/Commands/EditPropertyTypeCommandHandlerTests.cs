using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Application.Features.PropertyType.Commands.Edit;
using RealStateApp.Infraestructure.Persistence.Contexts;
using RealStateApp.Infraestructure.Persistence.Repositories;


namespace RealStateApp.Unit.Tests.Features.PropertyType.Commands
{
    public class EditPropertyTypeCommandHandlerTests
    {
        private readonly DbContextOptions<RealStateContext> _dbContextOptions;

        public EditPropertyTypeCommandHandlerTests()
        {

            _dbContextOptions = new DbContextOptionsBuilder<RealStateContext>()
                  .UseInMemoryDatabase(databaseName: $"RealStateAppTest_{Guid.NewGuid()}")
                  .Options;
        }



        [Fact]
        public async Task Handle_Should_UpdateImprovement_When_PropertyTypeExists()
        {
            // Arrange
            using var context = new RealStateContext(_dbContextOptions);
            var existingEntity = new Core.Domain.Entities.PropertyType
            {
                Id = 1,
                Name = "Old Name",
                Description = "Old Description"
            };
            context.PropertyTypes.Add(existingEntity);
            await context.SaveChangesAsync();

            var repository = new PropertyTypeRepository(context);
            var handler = new EditPropertyTypeCommandHandler(repository);

            var command = new EditPropertyTypeCommand
            {
                Id = 1,
                Name = "Updated Name",
                Description = "Updated Description"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(MediatR.Unit.Value);
            var updated = await context.PropertyTypes.FindAsync(command.Id);
            updated.Should().NotBeNull();
            updated!.Name.Should().Be(command.Name);
            updated.Description.Should().Be(command.Description);
        }

        [Fact]
        public async Task Handle_Should_ThrowException_When_PropertyType_DoesNotExist()
        {
            // Arrange
            using var context = new RealStateContext(_dbContextOptions);
            var repository = new PropertyTypeRepository(context);
            var handler = new EditPropertyTypeCommandHandler(repository);

            var command = new EditPropertyTypeCommand
            {
                Id = 999,
                Name = "New Name",
                Description = "New Description"
            };

            // Act
            Func<Task> act = () => handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ApiException>()
                .WithMessage("Property type not found with this id");
        }
    }
}
