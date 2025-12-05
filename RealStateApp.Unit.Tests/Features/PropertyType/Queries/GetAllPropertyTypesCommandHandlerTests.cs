using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealStateApp.Core.Application.Features.PropertyType.Queries.GetAll;
using RealStateApp.Core.Application.Features.SaleType.Queries.GetAll;
using RealStateApp.Core.Application.Mappings.EntitiesAndDtos;
using RealStateApp.Infraestructure.Persistence.Contexts;
using RealStateApp.Infraestructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Unit.Tests.Features.PropertyType.Queries
{
    public class GetAllPropertyTypesCommandHandlerTests
    {

        private readonly DbContextOptions<RealStateContext> _dbOptions;
        private readonly IMapper _mapper;
        public GetAllPropertyTypesCommandHandlerTests()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PropertyTypeToDtoMappingProfile>();
            }, loggerFactory);
            _mapper = mapperConfig.CreateMapper();

            _dbOptions = new DbContextOptionsBuilder<RealStateContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task Handle_ShouldReturnList_WhenPropertyTypesExist()
        {
            // Arrange
            using var context = new RealStateContext(_dbOptions);

            context.PropertyTypes.AddRange(
                new Core.Domain.Entities.PropertyType { Id = 1, Name = "property type1", Description = "Description1" },
                new Core.Domain.Entities.PropertyType { Id = 2, Name = "property type2", Description = "Description2" }
            );
            await context.SaveChangesAsync();

            var repository = new PropertyTypeRepository(context);
            var handler = new GetAllPropertyTypeQueryHandler(repository, _mapper);

            // Act
            var result = await handler.Handle(new GetAllPropertyTypeQuery(), CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result.Select(r => r.Name).Should().Contain(["property type1", "property type2"]);
        }
    }
}
