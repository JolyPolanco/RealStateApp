using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Application.Features.Improvement.Commands.EditImprovement;
using RealStateApp.Core.Application.Features.SaleType.Commands.EditSaleType;
using RealStateApp.Infraestructure.Persistence.Contexts;
using RealStateApp.Infraestructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Unit.Tests.Features.Improvement.Commands
{
    public class EditImprovementCommandHandlerTests
    {
        private readonly DbContextOptions<RealStateContext> _dbContextOptions;

        public EditImprovementCommandHandlerTests()
        {

            _dbContextOptions = new DbContextOptionsBuilder<RealStateContext>()
                  .UseInMemoryDatabase(databaseName: $"RealStateAppTest_{Guid.NewGuid()}")
                  .Options;
        }



        [Fact]
        public async Task Handle_Should_UpdateImprovement_When_ImprovementExists()
        {
            // Arrange
            using var context = new RealStateContext(_dbContextOptions);
            var existingEntity = new Core.Domain.Entities.Improvement
            {
                Id = 1,
                Name = "Old Name",
                Description = "Old Description"
            };
            context.Improvements.Add(existingEntity);
            await context.SaveChangesAsync();

            var repository = new ImprovementRepository(context);
            var handler = new EditImprovementCommandHandler(repository);

            var command = new EditImprovementCommand
            {
                Id = 1,
                Name = "Updated Name",
                Description = "Updated Description"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(MediatR.Unit.Value);
            var updated = await context.Improvements.FindAsync(command.Id);
            updated.Should().NotBeNull();
            updated!.Name.Should().Be(command.Name);
            updated.Description.Should().Be(command.Description);
        }

        [Fact]
        public async Task Handle_Should_ThrowException_When_Improvement_DoesNotExist()
        {
            // Arrange
            using var context = new RealStateContext(_dbContextOptions);
            var repository = new ImprovementRepository(context);
            var handler = new EditImprovementCommandHandler(repository);

            var command = new EditImprovementCommand
            {
                Id = 999,
                Name = "New Name",
                Description = "New Description"
            };

            // Act
            Func<Task> act = () => handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ApiException>()
                .WithMessage("Entity not found with Id");
        }
    }
}
