using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealStateApp.Core.Application.Dtos.Improvement;
using RealStateApp.Core.Application.Dtos.SaleType;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Application.Mappings.EntitiesAndDtos;
using RealStateApp.Core.Application.Services;
using RealStateApp.Core.Domain.Entities;
using RealStateApp.Infraestructure.Identity.Contexts;
using RealStateApp.Infraestructure.Identity.Entities;
using RealStateApp.Infraestructure.Persistence.Contexts;
using RealStateApp.Infraestructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Unit.Tests.Services
{
    public class ImprovementServiceTests
    {
        private readonly DbContextOptions<IdentityContext> _IdentitydbOptions;
        private readonly IMapper _mapper;
        private readonly DbContextOptions<RealStateContext> _dbOptions;

        public ImprovementServiceTests()
        {
            _dbOptions = new DbContextOptionsBuilder<RealStateContext>()
                .UseInMemoryDatabase($"RealStateDb_{Guid.NewGuid()}")
                .Options;

            var logger = LoggerFactory.Create(b => b.AddConsole());
            var mapping = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ImprovementToDtoMappingProfile>();
            }, logger);
            _IdentitydbOptions = new DbContextOptionsBuilder<IdentityContext>()
             .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
             .Options;
            _mapper = mapping.CreateMapper();
        }

        private ImprovementService CreateService()
        {
            var context = new RealStateContext(_dbOptions);
            var repo = new ImprovementRepository(context);
            return new ImprovementService(repo, _mapper);
        }


        [Fact]
        public async Task AddAsync_Should_ReturnDto_When_Success()
        {
            var service = CreateService();

            var dto = new ImprovementDto
            {
                Name = "Test",
                Description = "Description"
            };

            var result = await service.AddAsync(dto);

            result.Should().NotBeNull();
            result!.Id.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task AddAsync_Should_ReturnNull_When_Exception()
        {
            var service = CreateService();
            ImprovementDto dto = null!;

            var result = await service.AddAsync(dto);

            result.Should().BeNull();
        }



        [Fact]
        public async Task UpdateAsync_Should_UpdateEntity_When_Exists()
        {
            var service = CreateService();

            var added = await service.AddAsync(new ImprovementDto
            {
                Name = "Initial",
                Description = "Initial"
            });

            added!.Name = "Modified";

            var updated = await service.UpdateAsync(added.Id, added);

            updated.Should().NotBeNull();
            updated!.Name.Should().Be("Modified");
        }

        [Fact]
        public async Task UpdateAsync_Should_ReturnNull_When_EntityNotFound()
        {
            var service = CreateService();

            var dto = new ImprovementDto
            {
                Id = 999,
                Name = "Ghost",
                Description = "None"
            };

            var result = await service.UpdateAsync(999, dto);

            result.Should().BeNull();
        }



        [Fact]
        public async Task DeleteAsync_Should_RemoveEntity()
        {
            var service = CreateService();
            var added = await service.AddAsync(new ImprovementDto
            {
                Name = "ToDelete",
                Description = "Temp"
            });

            await service.DeleteAsync(added!.Id);
            var deleted = await service.GetByIdAsync(added.Id);

            deleted.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_Should_Throw_When_NotFound()
        {
            var service = CreateService();

            Func<Task> act = async () => await service.DeleteAsync(999);

            await act.Should()
                .ThrowAsync<ApiException>()
                .WithMessage("Entity not found with this id");
        }



        [Fact]
        public async Task GetByIdAsync_Should_ReturnDto_When_Exists()
        {
            var service = CreateService();
            var added = await service.AddAsync(new ImprovementDto
            {
                Name = "Type1",
                Description = "Test"
            });

            var result = await service.GetByIdAsync(added!.Id);

            result.Should().NotBeNull();
            result!.Name.Should().Be("Type1");
        }

        [Fact]
        public async Task GetByIdAsync_Should_ReturnNull_When_NotFound()
        {
            var service = CreateService();

            var result = await service.GetByIdAsync(500);

            result.Should().BeNull();
        }



        [Fact]
        public async Task GetAllList_Should_ReturnAllData()
        {
            var service = CreateService();

            await service.AddAsync(new ImprovementDto { Name = "A", Description = "1" });
            await service.AddAsync(new ImprovementDto { Name = "B", Description = "2" });

            var list = await service.GetAllList();

            list.Should().NotBeNull();
            list!.Count.Should().Be(2);
        }

        [Fact]
        public async Task GetAllList_Should_ReturnEmpty_When_NoData()
        {
            var service = CreateService();

            var list = await service.GetAllList();

            list!.Count.Should().Be(0);
        }


        [Fact]
        public async Task GetAllListWithInclude_Should_ReturnList()
        {
            var service = CreateService();

            await service.AddAsync(new ImprovementDto { Name = "A", Description = "1" });

            var result = await service.GetAllListWithInclude(new List<string> { "PropertyImprovements" });

            result.Should().NotBeNull();
            result!.Count.Should().Be(1);
        }



        [Fact]
        public async Task UpdateRangeAsync_Should_UpdateMultipleEntities()
        {
            var service = CreateService();

            var a = await service.AddAsync(new ImprovementDto { Name = "A", Description = "1" });
            var b = await service.AddAsync(new ImprovementDto { Name = "B", Description = "2" });

            a!.Name = "A1";
            b!.Name = "B1";
            service = CreateService();

            await service.UpdateRangeAsync(new List<ImprovementDto> { a, b });

            var all = await service.GetAllList();

            all!.Select(x => x.Name).Should().Contain(new[] { "A1", "B1" });
        }



        [Fact]
        public async Task DeleteRangeAsync_Should_RemoveMultiple()
        {
            var service = CreateService();

            var a = await service.AddAsync(new ImprovementDto { Name = "A", Description = "1" });
            var b = await service.AddAsync(new ImprovementDto { Name = "B", Description = "2" });
            service = CreateService();

            await service.DeleteRangeAsync(new List<ImprovementDto> { a!, b! });

            var all = await service.GetAllList();

            all!.Count.Should().Be(0);
        }






        [Fact]
        public async Task GetByIdAsync_Should_ReturnDto_When_EntityExists()
        {
            // Arrange
            var service = CreateService();

            var added = await service.AddAsync(new ImprovementDto
            {
                Name = "SaleTest",
                Description = "Test Description"
            });

            // Act
            var result = await service.GetByIdAsync(added!.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(added.Id);
            result.Name.Should().Be("SaleTest");
        }

        [Fact]
        public async Task GetByIdAsync_Should_ReturnNull_When_EntityDoesNotExist()
        {
            var service = CreateService();

            var result = await service.GetByIdAsync(500);

            result.Should().BeNull();
        }




        [Fact]
        public async Task GetAllList_Should_Return_All_SaleTypes()
        {
            // Arrange
            var service = CreateService();

            await service.AddAsync(new ImprovementDto { Name = "Type1", Description = "D1" });
            await service.AddAsync(new ImprovementDto { Name = "Type2", Description = "D2" });

            // Act
            var result = await service.GetAllList();

            // Assert
            result.Should().NotBeNull();
            result!.Count.Should().Be(2);
            result.Select(x => x.Name).Should().Contain(new[] { "Type1", "Type2" });
        }

        [Fact]
        public async Task GetAllList_Should_Return_Empty_When_NoData()
        {
            // Arrange
            var service = CreateService();

            // Act
            var result = await service.GetAllList();

            // Assert
            result.Should().NotBeNull();
            result!.Count.Should().Be(0);
        }





        [Fact]
        public async Task GetAllWithInclude_ShouldReturnImprovementDtos_WithCorrectPropertyCount()
        {
            // Arrange
            using var context = new RealStateContext(_dbOptions);

            // Improvements
            var improvement1 = new Improvement { Id = 1, Name = "Imp 1", Description = "D1" };
            var improvement2 = new Improvement { Id = 2, Name = "Imp 2", Description = "D2" };

            context.Improvements.AddRange(improvement1, improvement2);
            await context.SaveChangesAsync();

            // Agente mínimo
            using var identityContext = new IdentityContext(_IdentitydbOptions);

            var agent = new AppUser
            {
                FirstName = "A",
                LastName = "B",
                UserName = "agent",
                Email = "agent@test.com",
                Dni = "12345678910"
            };

            identityContext.Users.Add(agent);
            await identityContext.SaveChangesAsync();

            // Propiedades
            var property1 = new RealStateApp.Core.Domain.Entities.Property
            {
                AgentId = agent.Id,
                PropertyTypeId = 1,
                SaleTypeId = 1,
                Description = "",
                Code = "",
                Bedrooms = 2,
                Bathrooms = 1,
                Price = 100
            };

            var property2 = new RealStateApp.Core.Domain.Entities.Property
            {
                AgentId = agent.Id,
                PropertyTypeId = 1,
                SaleTypeId = 1,
                Description = "",
                Code = "",
                Bedrooms = 3,
                Bathrooms = 2,
                Price = 200
            };

            var property3 = new RealStateApp.Core.Domain.Entities.Property
            {
                AgentId = agent.Id,
                PropertyTypeId = 1,
                SaleTypeId = 1,
                Bedrooms = 1,
                Bathrooms = 1,
                Description = "",
                Code = "",
                Price = 50
            };

            context.Properties.AddRange(property1, property2, property3);
            await context.SaveChangesAsync();

            // PropertyImprovements (relaciones)
            context.PropertyImprovements.AddRange(
                new PropertyImprovement { PropertyId = property1.Id, ImprovementId = 1 },
                new PropertyImprovement { PropertyId = property2.Id, ImprovementId = 1 },
                new PropertyImprovement { PropertyId = property3.Id, ImprovementId = 2 }
            );

            await context.SaveChangesAsync();

            // Repositorio y servicio
            var improvementRepository = new ImprovementRepository(context);
            var service = new ImprovementService(improvementRepository, _mapper);

            // Act
            var result = await service.GetAllListWithInclude(new List<string> { "PropertyImprovements" });

            // Assert
            result.Should().HaveCount(2);

            var dto1 = result.First(x => x.Id == 1);
            dto1.PropertiesCount.Should().Be(2); // property1 & property2

            var dto2 = result.First(x => x.Id == 2);
            dto2.PropertiesCount.Should().Be(1); // property3

            result.Select(r => r.Name)
                  .Should()
                  .Contain(new[] { "Imp 1", "Imp 2" });
        }


        [Fact]
        public async Task GetAllListWithInclude_Should_ReturnEmpty_When_NoData()
        {
            // Arrange
            var service = CreateService();

            // Act
            var result = await service.GetAllListWithInclude(new List<string> { "PropertyImprovements" });

            // Assert
            result.Should().NotBeNull();
            result!.Count.Should().Be(0);
        }

        [Fact]
        public async Task GetAllListWithInclude_Should_Handle_EmptyIncludeList()
        {
            // Arrange
            var service = CreateService();

            await service.AddAsync(new ImprovementDto
            {
                Name = "TypeX",
                Description = "DescX"
            });

            // Act
            var result = await service.GetAllListWithInclude(new List<string>());

            // Assert
            result.Should().NotBeNull();
            result!.Count.Should().Be(1);
        }
    }
}
