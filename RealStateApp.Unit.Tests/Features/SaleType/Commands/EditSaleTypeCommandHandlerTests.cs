using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Application.Features.SaleType.Commands.EditSaleType;
using RealStateApp.Infraestructure.Persistence.Contexts;
using RealStateApp.Infraestructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Unit.Tests.Features.SaleType.Commands
{
    public class EditSaleTypeCommandHandlerTests
    {
        private readonly DbContextOptions<RealStateContext> _dbContextOptions;

        public EditSaleTypeCommandHandlerTests() {

            _dbContextOptions = new DbContextOptionsBuilder<RealStateContext>()
                  .UseInMemoryDatabase(databaseName: $"RealStateAppTest_{Guid.NewGuid()}")
                  .Options;
        }



         [Fact]
        public async Task Handle_Should_UpdateSaleType_When_SaleTypeExists()
        {
            // Arrange
            using var context = new RealStateContext(_dbContextOptions);
            var existingAssetType = new Core.Domain.Entities.SaleType
            {
                Id = 1,
                Name = "Old Name",
                Description = "Old Description"
            };
            context.SaleTypes.Add(existingAssetType);
            await context.SaveChangesAsync();

            var repository = new SaleTypeRepository(context);
            var handler = new EditSaleTypeCommandHandler(repository);

            var command = new EditSaleTypeCommand
            {
                Id = 1,
                Name = "Updated Name",
                Description = "Updated Description"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(MediatR.Unit.Value);
            var updated = await context.SaleTypes.FindAsync(command.Id);
            updated.Should().NotBeNull();
            updated!.Name.Should().Be(command.Name);
            updated.Description.Should().Be(command.Description);
        }

        [Fact]
        public async Task Handle_Should_ThrowException_When_SaleTypeDoesNotExist()
        {
            // Arrange
            using var context = new RealStateContext(_dbContextOptions);
            var repository = new SaleTypeRepository(context);
            var handler = new EditSaleTypeCommandHandler(repository);

            var command = new EditSaleTypeCommand
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
