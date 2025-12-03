using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Application.Features.SaleType.Commands.DeleteSaleType;
using RealStateApp.Infraestructure.Persistence.Contexts;
using RealStateApp.Infraestructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Unit.Tests.Features.SaleType.Commands
{
    public class DeleteSaleTypeCommandHandlerTests
    {
        private readonly DbContextOptions<RealStateContext> _dbContextOptions;
        public DeleteSaleTypeCommandHandlerTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<RealStateContext>()
              .UseInMemoryDatabase(databaseName: $"RealStateAppTest_{Guid.NewGuid()}")
              .Options;
        }

        [Fact]
        public async Task Handle_ShouldDeleteSaleType_WhenIdExists()
        {
            // Arrange
            using var context = new RealStateContext(_dbContextOptions);
            var existingAssetType = new Core.Domain.Entities.SaleType
            {
                Id = 1,
                Name = "ToDelete",
                Description = "Should be deleted"
            };
            context.SaleTypes.Add(existingAssetType);
            await context.SaveChangesAsync();

            var repository = new SaleTypeRepository (context);
            var handler = new DeleteSaleTypeCommandHandler (repository);
            var command = new DeleteSaleTypeCommand { Id = existingAssetType.Id };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(MediatR.Unit.Value);
            var deleted = await context.SaleTypes.FindAsync(existingAssetType.Id);
            deleted.Should().BeNull();
        }



        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenSaleTypeNotFound()
        {
            // Arrange
            using var context = new RealStateContext(_dbContextOptions);
            var repository = new SaleTypeRepository(context);
            var handler = new DeleteSaleTypeCommandHandler(repository);
            var command = new DeleteSaleTypeCommand { Id = 999 };

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ApiException>()
                .WithMessage("Entity  not found with this id");
        }
    }


    }

