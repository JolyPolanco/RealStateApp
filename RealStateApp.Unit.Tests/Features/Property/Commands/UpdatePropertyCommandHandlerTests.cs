using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Application.Features.Property.Commands.UpdateProperty;
using RealStateApp.Core.Domain.Common.Enums;
using RealStateApp.Infraestructure.Persistence.Contexts;
using RealStateApp.Infraestructure.Persistence.Repositories;

namespace RealStateApp.Unit.Tests.Features.Property.Commands
{
    public class UpdatePropertyCommandHandlerTests
    {
        private readonly DbContextOptions<RealStateContext> _dbContextOptions;

        public UpdatePropertyCommandHandlerTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<RealStateContext>()
                .UseInMemoryDatabase(databaseName: $"RealStateAppTest_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task Handle_ShouldUpdateProperty_WhenPropertyExists()
        {
            // Arrange
            using var context = new RealStateContext(_dbContextOptions);
            
            var propertyType1 = new Core.Domain.Entities.PropertyType { Id = 1, Name = "Casa", Description = "Casa" };
            var propertyType2 = new Core.Domain.Entities.PropertyType { Id = 2, Name = "Apartamento", Description = "Apartamento" };
            var saleType1 = new Core.Domain.Entities.SaleType { Id = 1, Name = "Venta", Description = "Venta" };
            var saleType2 = new Core.Domain.Entities.SaleType { Id = 2, Name = "Alquiler", Description = "Alquiler" };
            
            await context.PropertyTypes.AddRangeAsync(propertyType1, propertyType2);
            await context.SaleTypes.AddRangeAsync(saleType1, saleType2);
            
            var property = new Core.Domain.Entities.Property
            {
                Code = "ABC123",
                Price = 100000,
                SizeInMeters = 150.5,
                Bedrooms = 3,
                Bathrooms = 2,
                Description = "Original description",
                PropertyTypeId = 1,
                SaleTypeId = 1,
                AgentId = "agent123",
                Status = PropertyStatus.Available
            };
            
            await context.Properties.AddAsync(property);
            await context.SaveChangesAsync();

            var propertyRepository = new PropertyRepository(context);
            var propertyTypeRepository = new PropertyTypeRepository(context);
            var saleTypeRepository = new SaleTypeRepository(context);
            var propertyImprovementRepository = new PropertyImprovementRepository(context);

            var handler = new UpdatePropertyCommandHandler(
                propertyRepository,
                propertyTypeRepository,
                saleTypeRepository,
                propertyImprovementRepository);

            var command = new UpdatePropertyCommand
            {
                Code = "ABC123",
                Price = 200000,
                SizeInMeters = 200,
                Bedrooms = 4,
                Bathrooms = 3,
                Description = "Updated description",
                PropertyTypeId = 2,
                SaleTypeId = 2,
                Status = (int)PropertyStatus.Sold,
                ImprovementIds = new List<int>()
            };

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            var updatedProperty = await context.Properties.FirstOrDefaultAsync(p => p.Code == "ABC123");
            updatedProperty.Should().NotBeNull();
            updatedProperty!.Price.Should().Be(200000);
            updatedProperty.SizeInMeters.Should().Be(200);
            updatedProperty.Bedrooms.Should().Be(4);
            updatedProperty.Bathrooms.Should().Be(3);
            updatedProperty.Description.Should().Be("Updated description");
            updatedProperty.PropertyTypeId.Should().Be(2);
            updatedProperty.SaleTypeId.Should().Be(2);
            updatedProperty.Status.Should().Be(PropertyStatus.Sold);
        }

        [Fact]
        public async Task Handle_ShouldThrowApiException_WhenPropertyDoesNotExist()
        {
            // Arrange
            using var context = new RealStateContext(_dbContextOptions);
            
            var propertyRepository = new PropertyRepository(context);
            var propertyTypeRepository = new PropertyTypeRepository(context);
            var saleTypeRepository = new SaleTypeRepository(context);
            var propertyImprovementRepository = new PropertyImprovementRepository(context);

            var handler = new UpdatePropertyCommandHandler(
                propertyRepository,
                propertyTypeRepository,
                saleTypeRepository,
                propertyImprovementRepository);

            var command = new UpdatePropertyCommand
            {
                Code = "NOEXST",
                Price = 200000,
                SizeInMeters = 200,
                Bedrooms = 4,
                Bathrooms = 3,
                Description = "Updated description",
                PropertyTypeId = 1,
                SaleTypeId = 1,
                Status = (int)PropertyStatus.Sold,
                ImprovementIds = new List<int>()
            };

            // Act & Assert
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ApiException>()
                .WithMessage("La propiedad no existe con ese código");
        }
    }
}
