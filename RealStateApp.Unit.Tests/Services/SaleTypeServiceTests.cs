using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealStateApp.Core.Application.Mappings.EntitiesAndDtos;
using RealStateApp.Core.Application.Services;
using RealStateApp.Infraestructure.Persistence.Contexts;
using RealStateApp.Infraestructure.Persistence.Repositories;

namespace RealStateApp.Unit.Tests.Services
{
    public class SaleTypeServiceTests
    {
        private readonly IMapper _mapper;
        private readonly DbContextOptions<RealStateContext> _dbContextOptions;


        public SaleTypeServiceTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<RealStateContext>()
                .UseInMemoryDatabase(databaseName: $"RealStateAppTest_{Guid.NewGuid()}")
                .Options;

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();  
            });
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<SaleTypeToDtoMappingProfile>();
            }, loggerFactory);
            _mapper= mappingConfig.CreateMapper();

        }

        private SaleTypeService CreateService()
        {
            var context = new RealStateContext(_dbContextOptions);
            var saleTypeRepository= new SaleTypeRepository (context);
            return new SaleTypeService(saleTypeRepository, _mapper);

        }
    }
}
