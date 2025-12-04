using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Application.Features.Improvement.Commands.DeleteImprovement;
using RealStateApp.Core.Application.Features.SaleType.Commands.DeleteSaleType;
using RealStateApp.Infraestructure.Persistence.Contexts;
using RealStateApp.Infraestructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Unit.Tests.Features.Improvement.Commands
{
    public class DeleteImprovementHandlerTests
    {
        private readonly DbContextOptions<RealStateContext> _dbContextOptions;
        public DeleteImprovementHandlerTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<RealStateContext>()
              .UseInMemoryDatabase(databaseName: $"RealStateAppTest_{Guid.NewGuid()}")
              .Options;
        }

        [Fact]
        public async Task Handle_ShouldDeleteImprovement_WhenIdExists()
        {
            // Arrange
            using var context = new RealStateContext(_dbContextOptions);
            var existingImprovement = new Core.Domain.Entities.Improvement
            {
                Id = 1,
                Name = "ToDelete",
                Description = "Should be deleted"
            };
            context.Improvements.Add(existingImprovement);
            await context.SaveChangesAsync();

            var repository = new ImprovementRepository(context);
            var handler = new DeleteImprovementCommandHandler(repository);
            var command = new DeleteImprovementCommand { Id = existingImprovement.Id };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(MediatR.Unit.Value);
            var deleted = await context.Improvements.FindAsync(existingImprovement.Id);
            deleted.Should().BeNull();
        }



        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenImprovementNotFound()
        {
            // Arrange
            using var context = new RealStateContext(_dbContextOptions);
            var repository = new ImprovementRepository(context);
            var handler = new DeleteImprovementCommandHandler(repository);
            var command = new DeleteImprovementCommand { Id = 999 };

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ApiException>()
                .WithMessage("Entity  not found with this id");
        }
    }

}
