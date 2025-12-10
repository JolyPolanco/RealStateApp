using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Application.Features.SaleType.Commands.CreateSaleType;
using RealStateApp.Core.Domain.Interfaces;
using RealStateApp.Infraestructure.Persistence.Contexts;
using RealStateApp.Infraestructure.Persistence.Repositories;


namespace RealStateApp.Unit.Tests.Features.SaleType.Commands
{
    public class CreateSaleTypeCommandHandlerTests
    {
        private readonly DbContextOptions<RealStateContext> _dbContextOptions;

        public CreateSaleTypeCommandHandlerTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<RealStateContext>()
                .UseInMemoryDatabase(databaseName: $"RealStateAppTest_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]

        public async Task Handle_ShouldReturnSaleTypeId_WhenCreationIsSuccessful()
        {
            using var context = new RealStateContext(_dbContextOptions);
            var repository = new SaleTypeRepository(context);

            CreateSaleTypeCommandHandler handler = new CreateSaleTypeCommandHandler(repository);

            var command = new CreateSaleTypeCommand()
            {
                Name = "This is a test sale Type",
                Description = "Test SaleType",
            };

            
            //Act
            var result = await handler.Handle(command, CancellationToken.None);


            //Assert
            result.Should().BeGreaterThan(0);
            var createdEntity = await context.SaleTypes.FindAsync(result);
            createdEntity.Should().NotBeNull();
            createdEntity!.Name.Should().Be(command.Name);
            createdEntity.Description.Should().Be(command.Description);
        }

        [Fact]
        public async Task Handle_ShouldReturnZero_WhenRepositoryReturnsNull()
        {
            // Arrange
            Mock<ISaleTypeRepository> _mockRepository = new();
            CreateSaleTypeCommandHandler _handler = new(_mockRepository.Object);

            var command = new CreateSaleTypeCommand
            {
                Name = "Sale Type",
                Description = "Sale Type description"
            };

            _mockRepository
                   .Setup(r => r.AddAsync(It.IsAny<Core.Domain.Entities.SaleType>()))
                   .ReturnsAsync((Core.Domain.Entities.SaleType?)null);

            // Act && Assert
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ApiException>().WithMessage("Error creating sale type");
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Core.Domain.Entities.SaleType>()), Times.Once);
        }
    }
}
