using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealStateApp.Core.Application.Features.Improvement.Queries.GetAll;
using RealStateApp.Core.Application.Features.SaleType.Queries.GetAll;
using RealStateApp.Core.Application.Mappings.EntitiesAndDtos;
using RealStateApp.Infraestructure.Persistence.Contexts;
using RealStateApp.Infraestructure.Persistence.Repositories;


namespace RealStateApp.Unit.Tests.Features.Improvement.Queries
{
    public class GetAllImprovementsQueryHandlerTests
    {
        private readonly DbContextOptions<RealStateContext> _dbOptions;
        private readonly IMapper _mapper;
        public GetAllImprovementsQueryHandlerTests()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ImprovementToDtoMappingProfile>();
            }, loggerFactory);
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

            context.Improvements.AddRange(
                new Core.Domain.Entities.Improvement { Id = 1, Name = "Improvement1", Description = "Description1" },
                new Core.Domain.Entities.Improvement { Id = 2, Name = "Improvement2", Description = "Description2" }
            );
            await context.SaveChangesAsync();

            var repository = new ImprovementRepository(context);
            var handler = new GetAllImprovementQueryHandler(repository, _mapper);

            // Act
            var result = await handler.Handle(new GetAllImprovementQuery(), CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result.Select(r => r.Name).Should().Contain(["Improvement1", "Improvement2"]);
        }
    }
}
