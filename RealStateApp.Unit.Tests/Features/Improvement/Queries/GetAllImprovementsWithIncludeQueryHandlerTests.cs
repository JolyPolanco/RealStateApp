using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using RealStateApp.Core.Application.Dtos.Improvement;
using RealStateApp.Core.Application.Features.Improvement.Queries.GetAllWithInclude;
using RealStateApp.Core.Domain.Entities;
using RealStateApp.Core.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class GetAllImprovementsWithIncludeQueryHandlerTests
{
    private readonly Mock<IImprovementRepository> _mockRepo;
    private readonly IMapper _mapper;

    public GetAllImprovementsWithIncludeQueryHandlerTests()
    {
        _mockRepo = new Mock<IImprovementRepository>();
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Improvement, ImprovementDto>();
        },loggerFactory);

        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task Handle_ReturnsImprovementDtoList_WhenDataExists()
    {
        // Arrange
        var improvements = new List<Improvement>
    {
        new Improvement { Id = 1, Name = "Piscina", Description = "Description1" },
        new Improvement { Id = 2, Name = "Jardín", Description = "Description1" }
    }.AsQueryable();

        _mockRepo
            .Setup(r => r.GetAllQueryWithInclude(It.IsAny<List<string>>()))
            .Returns(improvements);

    

        var handler = new GetAllWithIncludeImprovementQueryHandler(_mockRepo.Object,_mapper);

        // Act
        var result = await handler.Handle(new GetAllImprovementWithIncludeQuery(), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, x => x.Name == "Piscina");
        Assert.Contains(result, x => x.Name == "Jardín");
    }

}
