using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealStateApp.Core.Application.Features.Improvement.Queries.GetAll;
using RealStateApp.Core.Application.Features.SaleType.Queries.GetAll;
using RealStateApp.Core.Application.Mappings.EntitiesAndDtos;
using RealStateApp.Infraestructure.Persistence.Contexts;
using RealStateApp.Infraestructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Unit.Tests.Features.SaleType.Queries
{
    public class GetAllSaleTypesQueryHandlerTests
    {
        private readonly DbContextOptions<RealStateContext> _dbOptions;
        private readonly IMapper _mapper;
        public GetAllSaleTypesQueryHandlerTests()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<SaleTypeToDtoMappingProfile>();
            },loggerFactory);
            _mapper = mapperConfig.CreateMapper();

            _dbOptions = new DbContextOptionsBuilder<RealStateContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task Handle_ShouldReturnList_WhenSaleTypesExist()
        {
            // Arrange
            using var context = new RealStateContext(_dbOptions);

            context.SaleTypes.AddRange(
                new Core.Domain.Entities.SaleType { Id = 1, Name = "Sale type1", Description = "Description1" },
                new Core.Domain.Entities.SaleType { Id = 2, Name = "Sale type2", Description = "Description1" }
            );
            await context.SaveChangesAsync();

            var repository = new SaleTypeRepository(context);
            var handler = new GetAllSaleTypeQueryHandler(repository, _mapper);

            // Act
            var result = await handler.Handle(new GetAllSaleTypeQuery(), CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result.Select(r => r.Name).Should().Contain(["Sale type1", "Sale type2"]);
        }
    }
}
