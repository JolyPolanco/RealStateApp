using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using RealStateApp.Core.Application.Dtos.Improvement;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Application.Features.Improvement.Queries.GetById;
using RealStateApp.Core.Application.Features.SaleType.Queries.GetById;
using RealStateApp.Core.Domain.Entities;
using RealStateApp.Core.Domain.Interfaces;
using RealStateApp.Infraestructure.Persistence.Contexts;
using RealStateApp.Infraestructure.Persistence.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class GetImprovementByIdQueryHandlerTests
{
    private readonly Mock<IImprovementRepository> _mockRepo;
    private readonly IMapper _mapper;

    public GetImprovementByIdQueryHandlerTests()
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
    public async Task Handle_ReturnsImprovementDto_WhenIdExists()
    {
        var options = new DbContextOptionsBuilder<RealStateContext>()
            .UseInMemoryDatabase(databaseName: "TestDB_Improvement")
            .Options;

        using var context = new RealStateContext(options);

        // Datos de prueba en memoria
        var improvement = new Improvement
        {
            Id = 1,
            Name = "Piscina",
            Description = "Desc"
        };

        context.Improvements.Add(improvement);
        await context.SaveChangesAsync();

        var repo = new ImprovementRepository(context);

        var handler = new GetImprovementByIdQueryHandler(repo, _mapper);

        // Act
        var result = await handler.Handle(new GetImprovementByIdQuery { Id = 1 }, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Piscina", result.Name);
    }


    [Fact]
    public async Task Handle_ShouldThrowApiException_WhenIdDoesNotExist()
    {
        // Arrange

        var options = new DbContextOptionsBuilder<RealStateContext>()
            .UseInMemoryDatabase(databaseName: "TestDB_Improvement")
            .Options;
        using var context = new RealStateContext(options);
        var repository = new ImprovementRepository(context);
        var handler = new GetImprovementByIdQueryHandler(repository, _mapper);

        var query = new GetImprovementByIdQuery { Id = 999 };

        // Act
        Func<Task> act = async () => await handler.Handle(query, CancellationToken.None);
        // Assert
        await act.Should().ThrowAsync<ApiException>()
            .WithMessage("Entity Not found with this id");
    }
}