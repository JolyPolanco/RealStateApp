using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealStateApp.Core.Application.Dtos.Properties;
using RealStateApp.Core.Application.Mappings.EntitiesAndDtos;
using RealStateApp.Core.Application.Services;
using RealStateApp.Core.Domain.Common.Enums;
using RealStateApp.Core.Domain.Entities;
using RealStateApp.Infraestructure.Persistence.Contexts;
using RealStateApp.Infraestructure.Persistence.Repositories;

namespace RealStateApp.Unit.Tests.Services
{
    public class PropertyServiceTests
    {
        private readonly DbContextOptions<RealStateContext> _dbOptions;
        private readonly IMapper _mapper;

        public PropertyServiceTests()
        {
            _dbOptions = new DbContextOptionsBuilder<RealStateContext>()
                .UseInMemoryDatabase($"RealStateDb_{Guid.NewGuid()}")
                .Options;

            var logger = LoggerFactory.Create(b => b.AddConsole());
            var mapping = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PropertyToDtoMappingProfile>();
            }, logger);
            _mapper = mapping.CreateMapper();
        }

        private PropertyService CreateService()
        {
            var context = new RealStateContext(_dbOptions);
            var repo = new PropertyRepository(context);
            return new PropertyService(repo, _mapper);
        }

        [Fact]
        public async Task GetAvailablePropertiesCount_Should_ReturnCorrectCount_When_PropertiesExist()
        {
            // Arrange
            var service = CreateService();
            using var context = new RealStateContext(_dbOptions);

            var propertyType = new PropertyType { Id = 1, Name = "Casa", Description = "Casa" };
            var saleType = new SaleType { Id = 1, Name = "Venta", Description = "Venta" };
            await context.PropertyTypes.AddAsync(propertyType);
            await context.SaleTypes.AddAsync(saleType);

            var properties = new List<Property>
            {
                new Property
                {
                    Code = "ABC123",
                    Price = 100000,
                    SizeInMeters = 150,
                    Bedrooms = 3,
                    Bathrooms = 2,
                    Description = "Available property 1",
                    PropertyTypeId = 1,
                    SaleTypeId = 1,
                    AgentId = "agent1",
                    Status = PropertyStatus.Available
                },
                new Property
                {
                    Code = "DEF456",
                    Price = 200000,
                    SizeInMeters = 200,
                    Bedrooms = 4,
                    Bathrooms = 3,
                    Description = "Available property 2",
                    PropertyTypeId = 1,
                    SaleTypeId = 1,
                    AgentId = "agent1",
                    Status = PropertyStatus.Available
                },
                new Property
                {
                    Code = "GHI789",
                    Price = 150000,
                    SizeInMeters = 180,
                    Bedrooms = 3,
                    Bathrooms = 2,
                    Description = "Sold property",
                    PropertyTypeId = 1,
                    SaleTypeId = 1,
                    AgentId = "agent1",
                    Status = PropertyStatus.Sold
                }
            };

            await context.Properties.AddRangeAsync(properties);
            await context.SaveChangesAsync();

            // Act
            var count = await service.GetAvailablePropertiesCount();

            // Assert
            count.Should().Be(2);
        }

        [Fact]
        public async Task GetAvailablePropertiesCount_Should_ReturnZero_When_NoAvailableProperties()
        {
            // Arrange
            var service = CreateService();
            using var context = new RealStateContext(_dbOptions);

            var propertyType = new PropertyType { Id = 1, Name = "Casa", Description = "Casa" };
            var saleType = new SaleType { Id = 1, Name = "Venta", Description = "Venta" };
            await context.PropertyTypes.AddAsync(propertyType);
            await context.SaleTypes.AddAsync(saleType);

            var property = new Property
            {
                Code = "ABC123",
                Price = 100000,
                SizeInMeters = 150,
                Bedrooms = 3,
                Bathrooms = 2,
                Description = "Sold property",
                PropertyTypeId = 1,
                SaleTypeId = 1,
                AgentId = "agent1",
                Status = PropertyStatus.Sold
            };

            await context.Properties.AddAsync(property);
            await context.SaveChangesAsync();

            // Act
            var count = await service.GetAvailablePropertiesCount();

            // Assert
            count.Should().Be(0);
        }

        [Fact]
        public async Task GetSoldPropertiesCount_Should_ReturnCorrectCount_When_PropertiesExist()
        {
            // Arrange
            var service = CreateService();
            using var context = new RealStateContext(_dbOptions);

            var propertyType = new PropertyType { Id = 1, Name = "Casa", Description = "Casa" };
            var saleType = new SaleType { Id = 1, Name = "Venta", Description = "Venta" };
            await context.PropertyTypes.AddAsync(propertyType);
            await context.SaleTypes.AddAsync(saleType);

            var properties = new List<Property>
            {
                new Property
                {
                    Code = "ABC123",
                    Price = 100000,
                    SizeInMeters = 150,
                    Bedrooms = 3,
                    Bathrooms = 2,
                    Description = "Sold property 1",
                    PropertyTypeId = 1,
                    SaleTypeId = 1,
                    AgentId = "agent1",
                    Status = PropertyStatus.Sold
                },
                new Property
                {
                    Code = "DEF456",
                    Price = 200000,
                    SizeInMeters = 200,
                    Bedrooms = 4,
                    Bathrooms = 3,
                    Description = "Sold property 2",
                    PropertyTypeId = 1,
                    SaleTypeId = 1,
                    AgentId = "agent1",
                    Status = PropertyStatus.Sold
                },
                new Property
                {
                    Code = "GHI789",
                    Price = 150000,
                    SizeInMeters = 180,
                    Bedrooms = 3,
                    Bathrooms = 2,
                    Description = "Available property",
                    PropertyTypeId = 1,
                    SaleTypeId = 1,
                    AgentId = "agent1",
                    Status = PropertyStatus.Available
                }
            };

            await context.Properties.AddRangeAsync(properties);
            await context.SaveChangesAsync();

            // Act
            var count = await service.GetSoldPropertiesCount();

            // Assert
            count.Should().Be(2);
        }

        [Fact]
        public async Task GetSoldPropertiesCount_Should_ReturnZero_When_NoSoldProperties()
        {
            // Arrange
            var service = CreateService();
            using var context = new RealStateContext(_dbOptions);

            var propertyType = new PropertyType { Id = 1, Name = "Casa", Description = "Casa" };
            var saleType = new SaleType { Id = 1, Name = "Venta", Description = "Venta" };
            await context.PropertyTypes.AddAsync(propertyType);
            await context.SaleTypes.AddAsync(saleType);

            var property = new Property
            {
                Code = "ABC123",
                Price = 100000,
                SizeInMeters = 150,
                Bedrooms = 3,
                Bathrooms = 2,
                Description = "Available property",
                PropertyTypeId = 1,
                SaleTypeId = 1,
                AgentId = "agent1",
                Status = PropertyStatus.Available
            };

            await context.Properties.AddAsync(property);
            await context.SaveChangesAsync();

            // Act
            var count = await service.GetSoldPropertiesCount();

            // Assert
            count.Should().Be(0);
        }

        [Fact]
        public async Task AddAsync_Should_ReturnDto_When_Success()
        {
            // Arrange
            using var setupContext = new RealStateContext(_dbOptions);

            var propertyType = new PropertyType { Id = 1, Name = "Casa", Description = "Casa" };
            var saleType = new SaleType { Id = 1, Name = "Venta", Description = "Venta" };
            await setupContext.PropertyTypes.AddAsync(propertyType);
            await setupContext.SaleTypes.AddAsync(saleType);
            await setupContext.SaveChangesAsync();

            var dto = new PropertyDto
            {
                Code = "TEST01",
                Price = 250000,
                SizeInMeters = 120,
                Bedrooms = 3,
                Bathrooms = 2,
                Description = "Test property",
                PropertyTypeId = 1,
                SaleTypeId = 1,
                AgentId = "agent1"
            };
            
            var service = CreateService();

            // Act
            var result = await service.AddAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().BeGreaterThan(0);
            result.Code.Should().Be("TEST01");
        }

        [Fact]
        public async Task AddAsync_Should_ReturnNull_When_Exception()
        {
            // Arrange
            var service = CreateService();
            PropertyDto dto = null!;

            // Act
            var result = await service.AddAsync(dto);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAsync_Should_UpdateProperty_When_PropertyExists()
        {
            // Arrange
            using var context = new RealStateContext(_dbOptions);

            var propertyType = new PropertyType { Id = 1, Name = "Casa", Description = "Casa" };
            var saleType = new SaleType { Id = 1, Name = "Venta", Description = "Venta" };
            await context.PropertyTypes.AddAsync(propertyType);
            await context.SaleTypes.AddAsync(saleType);

            var property = new Property
            {
                Code = "UPD001",
                Price = 100000,
                SizeInMeters = 150,
                Bedrooms = 3,
                Bathrooms = 2,
                Description = "Original description",
                PropertyTypeId = 1,
                SaleTypeId = 1,
                AgentId = "agent1",
                Status = PropertyStatus.Available
            };

            await context.Properties.AddAsync(property);
            await context.SaveChangesAsync();

            var updateDto = new PropertyDto
            {
                Id = property.Id,
                Code = "UPD001",
                Price = 200000,
                SizeInMeters = 180,
                Bedrooms = 4,
                Bathrooms = 3,
                Description = "Updated description",
                PropertyTypeId = 1,
                SaleTypeId = 1,
                AgentId = "agent1"
            };
            
            var service = CreateService();

            // Act
            await service.UpdateAsync(property.Id, updateDto);

            // Assert
            // Create new context to verify update
            using var verifyContext = new RealStateContext(_dbOptions);
            var updated = await verifyContext.Properties.FindAsync(property.Id);
            updated.Should().NotBeNull();
            updated!.Price.Should().Be(200000);
            updated.Description.Should().Be("Updated description");
        }

        [Fact]
        public async Task GetAllList_Should_ReturnAllProperties()
        {
            // Arrange
            var service = CreateService();
            using var context = new RealStateContext(_dbOptions);

            var propertyType = new PropertyType { Id = 1, Name = "Casa", Description = "Casa" };
            var saleType = new SaleType { Id = 1, Name = "Venta", Description = "Venta" };
            await context.PropertyTypes.AddAsync(propertyType);
            await context.SaleTypes.AddAsync(saleType);

            var properties = new List<Property>
            {
                new Property
                {
                    Code = "PROP01",
                    Price = 100000,
                    SizeInMeters = 150,
                    Bedrooms = 3,
                    Bathrooms = 2,
                    Description = "Property 1",
                    PropertyTypeId = 1,
                    SaleTypeId = 1,
                    AgentId = "agent1",
                    Status = PropertyStatus.Available
                },
                new Property
                {
                    Code = "PROP02",
                    Price = 200000,
                    SizeInMeters = 200,
                    Bedrooms = 4,
                    Bathrooms = 3,
                    Description = "Property 2",
                    PropertyTypeId = 1,
                    SaleTypeId = 1,
                    AgentId = "agent1",
                    Status = PropertyStatus.Sold
                }
            };

            await context.Properties.AddRangeAsync(properties);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetAllList();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task DeleteAsync_Should_RemoveProperty_When_PropertyExists()
        {
            // Arrange
            using var context = new RealStateContext(_dbOptions);
            
            var propertyType = new PropertyType { Id = 1, Name = "Casa", Description = "Casa" };
            var saleType = new SaleType { Id = 1, Name = "Venta", Description = "Venta" };
            await context.PropertyTypes.AddAsync(propertyType);
            await context.SaleTypes.AddAsync(saleType);

            var property = new Property
            {
                Code = "DEL001",
                Price = 100000,
                SizeInMeters = 150,
                Bedrooms = 3,
                Bathrooms = 2,
                Description = "To be deleted",
                PropertyTypeId = 1,
                SaleTypeId = 1,
                AgentId = "agent1",
                Status = PropertyStatus.Available
            };

            await context.Properties.AddAsync(property);
            await context.SaveChangesAsync();
            
            var service = CreateService();

            // Act
            await service.DeleteAsync(property.Id);

            // Assert
            // Create new context to verify deletion
            using var verifyContext = new RealStateContext(_dbOptions);
            var deleted = await verifyContext.Properties.FindAsync(property.Id);
            deleted.Should().BeNull();
        }
    }
}
