

using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using RealStateApp.Core.Application.Dtos.Offer;
using RealStateApp.Core.Application.Mappings.DtosAndViewModels;
using RealStateApp.Core.Application.Mappings.EntitiesAndDtos;
using RealStateApp.Core.Application.Services;
using RealStateApp.Core.Domain.Common.Enums;
using RealStateApp.Core.Domain.Entities;
using RealStateApp.Core.Domain.Interfaces;
using RealStateApp.Infraestructure.Identity.Contexts;
using RealStateApp.Infraestructure.Identity.Entities;
using RealStateApp.Infraestructure.Persistence.Contexts;
using RealStateApp.Infraestructure.Persistence.Repositories;

namespace RealStateApp.Unit.Tests.Services
{
    public class OfferServiceTests
    {


        private readonly IMapper mapper;
        private readonly DbContextOptions<RealStateContext> dbContextOptions;
        private readonly DbContextOptions<IdentityContext> IdentityContextOptions;
        private readonly Mock<IOfferRepository> MockOffer;


        public OfferServiceTests()
        {


            dbContextOptions = new DbContextOptionsBuilder<RealStateContext>()
               .UseInMemoryDatabase(databaseName: $"dbTestOfferService_{Guid.NewGuid()}")
               .Options;



            IdentityContextOptions = new DbContextOptionsBuilder<IdentityContext>()
            .UseInMemoryDatabase(databaseName: $"dbIdentityTestOfferService_{Guid.NewGuid()}")
            .Options;

            MockOffer = new Mock<IOfferRepository>();

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });


            var config = new MapperConfiguration(cf =>
            {
                cf.AddProfile<OfferDtosAndViewModelMappingProfile>();
                cf.AddProfile<OfferToDtoMappingProfile>();
            }, loggerFactory);


            mapper = config.CreateMapper();

        }






        private OfferService CreateService()
        {

            var context = new RealStateContext(dbContextOptions);
            var repo = new OfferRepository(context);

            var service = new OfferService(repo, mapper);
            return service;

        }


        #region private method creacion de usuario
        private async Task<List<AppUser>> CreateUsers()
        {
            var passwordHasher = new PasswordHasher<AppUser>();
            var Identitycontext = new IdentityContext(IdentityContextOptions);

            var users = new List<AppUser>
            {

                new() { Id = Guid.NewGuid().ToString(), FirstName = "Kelvin", LastName = "Ramirez", Email = "Kelvin@gmail.com", EmailConfirmed = true, IsActive = true, PhoneNumber = "58930003", UserName = "KIONoudes"},
                new() { Id = Guid.NewGuid().ToString(), FirstName = "Kodak", LastName = "Diaz", Email = "Kodak@gmail.com", EmailConfirmed = true, IsActive = true, PhoneNumber = "58930003", UserName = "KIONoudes", },

            };


            foreach (var user in users)
            {

                int d = 1;
                user.PasswordHash = passwordHasher.HashPassword(user, user.PasswordHash = $"@kelvn3{d++}");
                d++;

            }


            await Identitycontext.Users.AddRangeAsync(users);
            await Identitycontext.SaveChangesAsync();
            var _UserInDatabase = await Identitycontext.Users.ToListAsync();

            //creamos roless
            var roles = new List<IdentityRole>
             {

                 new()  { Id = Guid.NewGuid().ToString(), Name = $"{AppRoles.AGENT.ToString()}", NormalizedName = $"{AppRoles.AGENT.ToString()}" },
                 new()  { Id = Guid.NewGuid().ToString(), Name = $"{AppRoles.CLIENT.ToString()}", NormalizedName = $"{AppRoles.CLIENT.ToString()}" },
             };



            await Identitycontext.Roles.AddRangeAsync(roles);
            await Identitycontext.SaveChangesAsync();
            var rolesInDatabase = await Identitycontext.Roles.ToListAsync();


            //asignamos roles
            var userRoles = new List<IdentityUserRole<string>>()
            {


                new() { RoleId = rolesInDatabase.FirstOrDefault(s => s.Name == "AGENT" )!.Id, UserId = _UserInDatabase.FirstOrDefault(s => s.FirstName == "Kelvin")!.Id},
                new() { RoleId = rolesInDatabase.FirstOrDefault(s => s.Name == "CLIENT")!.Id, UserId = _UserInDatabase.FirstOrDefault(s => s.FirstName == "Kodak")!.Id},
            };


            await Identitycontext.UserRoles.AddRangeAsync(userRoles);
            await Identitycontext.SaveChangesAsync();
            var userRolesInDatabase = await Identitycontext.UserRoles.ToListAsync();
            return _UserInDatabase;

        }
        #endregion

        #region CreateProperty private method 
        public async Task<(List<Property>, List<AppUser>)> CreateProperty()
        {
            var context = new RealStateContext(dbContextOptions);
            var _UserInDatabase = await CreateUsers();

            //creamos tipo de propiedad
            var PropertyTypeRepo = new PropertyTypeRepository(context);
            var propertyType = await PropertyTypeRepo.AddAsync(new PropertyType() { Id = 0, Name = "Apartamento", Description = "Es una partamento amueblado...." });


            //creamos tipo de ventas
            var SaleTypeRepo = new SaleTypeRepository(context);
            var saleType = await SaleTypeRepo.AddAsync(new SaleType() { Id = 0, Name = "Venta", Description = "Venta de apartamento full, negociable" });


            //creamos mejoras que se le puede hacer a la propiedad
            var ImprovementRepo = new ImprovementRepository(context);
            var Improvements = new List<Improvement>
            {

                new() { Id = 0, Name = "Cocina", Description = "Se require hacer arreglo minimo en la cocina, por filtro de ...."},
                new() { Id = 0, Name = "Baño", Description = "Se require hacer arreglo minimo en la cocina, por filtro de agua...."},
                new() { Id = 0, Name = "Area de lavado", Description = "Se requiere cambiar algunas tuverias..."}


            };

            await ImprovementRepo.AddRangeAsync(Improvements);
            var entitiesImprovement = await ImprovementRepo.GetAllList();


            //creamo la propiedad
            var properties = new List<Property>
            {
                new()
                {

                Id = 0,
                AgentId = _UserInDatabase.FirstOrDefault(s => s.FirstName == "Kelvin")!.Id,
                Bathrooms = 3,
                Bedrooms = 6,
                Code = "P09DDOD",
                Description = "Apartamento con opcion de compra negociable.....",
                Price = 1500000000,
                SizeInMeters = 150.90,
                SaleTypeId = saleType.Id,
                PropertyTypeId = propertyType.Id,
                Status = PropertyStatus.Available

                },
                new()
                {

                 Id = 0,
                 AgentId = _UserInDatabase.FirstOrDefault(s => s.FirstName == "Kelvin")!.Id,
                 Bathrooms = 2,
                 Bedrooms = 6,
                 Code = "00PDOK",
                 Description = "A una casa para arquiler muy amplia y con opcion de compra.....",
                 Price = 100000000,
                 SizeInMeters = 150.90,
                 SaleTypeId = saleType.Id,
                 PropertyTypeId = propertyType.Id,
                 Status = PropertyStatus.Available
                },
                new()
                {


                 Id = 0,
                 AgentId = _UserInDatabase.FirstOrDefault(s => s.FirstName == "Kelvin")!.Id,
                 Bathrooms = 5,
                 Bedrooms = 10,
                 Code = "MAP-3920",
                 Description = "A Una mansion.....",
                 Price = 400000000,
                 SizeInMeters = 150.90,
                 SaleTypeId = saleType.Id,
                 PropertyTypeId = propertyType.Id,
                 Status = PropertyStatus.Available

                },


            };

            var repo = new PropertyRepository(context);
            await repo.AddRangeAsync(properties);
            var ListPropery = await repo.GetAllList();

            //asignamos mejoras a la propiedad

            var PropertyImprovement = new List<PropertyImprovement>
            {

                new() { ImprovementId = entitiesImprovement!.FirstOrDefault(s => s.Name == "Cocina")!.Id, PropertyId = ListPropery!.FirstOrDefault(s => s.Code == "P09DDOD")!.Id},
                new() { ImprovementId = entitiesImprovement!.FirstOrDefault(s => s.Name == "Baño")!.Id, PropertyId =ListPropery!.FirstOrDefault(s => s.Code == "P09DDOD")!.Id},
                new() { ImprovementId = entitiesImprovement!.FirstOrDefault(s => s.Name == "Area de lavado")!.Id, PropertyId =ListPropery!.FirstOrDefault(s => s.Code == "P09DDOD")!.Id},



                new() { ImprovementId = entitiesImprovement!.FirstOrDefault(s => s.Name == "Cocina")!.Id, PropertyId = ListPropery!.FirstOrDefault(s => s.Code == "00PDOK")!.Id},
                new() { ImprovementId = entitiesImprovement!.FirstOrDefault(s => s.Name == "Baño")!.Id, PropertyId =ListPropery!.FirstOrDefault(s => s.Code == "00PDOK")!.Id},
                new() { ImprovementId = entitiesImprovement!.FirstOrDefault(s => s.Name == "Area de lavado")!.Id, PropertyId =ListPropery!.FirstOrDefault(s => s.Code == "00PDOK")!.Id},



                new() { ImprovementId = entitiesImprovement!.FirstOrDefault(s => s.Name == "Cocina")!.Id, PropertyId = ListPropery!.FirstOrDefault(s => s.Code == "MAP-3920")!.Id},
                new() { ImprovementId = entitiesImprovement!.FirstOrDefault(s => s.Name == "Baño")!.Id, PropertyId =ListPropery!.FirstOrDefault(s => s.Code == "MAP-3920")!.Id},
                new() { ImprovementId = entitiesImprovement!.FirstOrDefault(s => s.Name == "Area de lavado")!.Id, PropertyId =ListPropery!.FirstOrDefault(s => s.Code == "MAP-3920")!.Id},

            };

            var PropertyImprovementRepo = new PropertyImprovementRepository(context);
            await PropertyImprovementRepo.AddRangeAsync(PropertyImprovement);
            return (ListPropery!, _UserInDatabase);
        }
        #endregion



        [Fact]
        public async Task AddAsync_Should_Return_Dto_When_Add()
        {

            //arrange
            var service = CreateService();

            (var propertyInDatabase, var UserInDatabase) = await CreateProperty();


            var offer = new CreateOfferDto()
            {

                Id = 0,
                Date = DateTime.Now,
                Amount = 1200000000,
                Status = OfferStatus.Pending,
                ClientId = UserInDatabase.First(s => s.FirstName == "Kodak").Id,
                PropertyId = propertyInDatabase.First(s => s.AgentId == UserInDatabase.First(s => s.FirstName == "Kelvin").Id).Id,

            };

            //act
            var result = await service.AddAsync(offer);

            //assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);

        }



        [Fact]
        public async Task AddAsync_Should_Return_null_When_Not_Add()
        {


            //arrange
            var service = CreateService();


            //act
            var result = await service.AddAsync(null!);

            //assert
            result.Should().BeNull();

        }




        [Fact]
        public async Task GetByIdAsync_Should_Return_Offer_When_Exist()
        {


            //arrange
            var service = CreateService();


            (var propertyInDatabase, var UserInDatabase) = await CreateProperty();


            var offer = new CreateOfferDto()
            {

                Id = 0,
                Date = DateTime.Now,
                Amount = 1200000000,
                Status = OfferStatus.Pending,
                ClientId = UserInDatabase.First(s => s.FirstName == "Kodak").Id,
                PropertyId = propertyInDatabase.First(s => s.AgentId == UserInDatabase.First(s => s.FirstName == "Kelvin").Id).Id,

            };


            //act
            var SaveOffer = await service.AddAsync(offer);
            var result = await service.GetByIdAsync(SaveOffer!.Id);

            //assert
            result.Should().NotBeNull();
            result.Id.Should().Be(SaveOffer.Id);
            result.Amount.Should().Be(SaveOffer.Amount);

        }


        [Fact]
        public async Task GetByIdAsync_Should_Return_null_When_Not_Exist()
        {


            //arrange
            var service = CreateService();

            //act
            var result = await service.GetByIdAsync(999);


            //assert
            result.Should().BeNull();


        }




        [Fact]
        public async Task DeleteAsync_Should_Return_null_When_Exist()
        {


            //arrange
            var service = CreateService();


            (var propertyInDatabase, var UserInDatabase) = await CreateProperty();


            var offer = new CreateOfferDto()
            {

                Id = 0,
                Date = DateTime.Now,
                Amount = 1200000000,
                Status = OfferStatus.Pending,
                ClientId = UserInDatabase.First(s => s.FirstName == "Kodak").Id,
                PropertyId = propertyInDatabase.First(s => s.AgentId == UserInDatabase.First(s => s.FirstName == "Kelvin").Id).Id,

            };


            //act
            var SaveOffer = await service.AddAsync(offer);
            await service.DeleteAsync(SaveOffer!.Id);
            var result = await service.GetByIdAsync(SaveOffer.Id);


            //assert
            result.Should().BeNull();


        }



        [Fact]
        public async Task DeleteAsync_Should_Return_Throw_When_Not_Exist()
        {
            // arrange
            MockOffer
            .Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Offer?)null);


            var service = new OfferService(MockOffer.Object, mapper);


            // act
            Func<Task> act = async () => await service.DeleteAsync(999);

            // assert
            await act.Should().ThrowAsync<Exception>();


        }




        [Fact]
        public async Task UpdateAsync_Should_Return_DTo_When_Updated()
        {
            var service = CreateService();


            (var propertyInDatabase, var UserInDatabase) = await CreateProperty();


            var offer = new CreateOfferDto()
            {

                Id = 0,
                Date = DateTime.Now,
                Amount = 1200000000,
                Status = OfferStatus.Pending,
                ClientId = UserInDatabase.First(s => s.FirstName == "Kodak").Id,
                PropertyId = propertyInDatabase.First(s => s.AgentId == UserInDatabase.First(s => s.FirstName == "Kelvin").Id).Id,

            };


            //act
            var SaveOffer = await service.AddAsync(offer);
            SaveOffer!.Amount = 30000000;
            var result = await service.UpdateAsync(SaveOffer.Id, SaveOffer);



            //Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(SaveOffer.Id);
            result.Amount.Should().Be(SaveOffer.Amount);


        }




        [Fact]
        public async Task UpdateAsync_Should_Return_null_When_Not_Updated()
        {
            var service = CreateService();


            (var propertyInDatabase, var UserInDatabase) = await CreateProperty();


            var offer = new CreateOfferDto()
            {

                Id = 0,
                Date = DateTime.Now,
                Amount = 1200000000,
                Status = OfferStatus.Pending,
                ClientId = UserInDatabase.First(s => s.FirstName == "Kodak").Id,
                PropertyId = propertyInDatabase.First(s => s.AgentId == UserInDatabase.First(s => s.FirstName == "Kelvin").Id).Id,

            };


            //act
            var result = await service.UpdateAsync(offer.Id, offer);


            //Assert
            result.Should().BeNull();

        }




        [Fact]
        public async Task GetAllLIst_Should_Return_All_Offer_When_Exists()
        {
            var service = CreateService();


            (var propertyInDatabase, var UserInDatabase) = await CreateProperty();


            var offers = new List<CreateOfferDto>
            {
               new(){
                Id = 0,
                Date = DateTime.Now,
                Amount = 1780000000,
                Status = OfferStatus.Pending,
                ClientId = UserInDatabase.First(s => s.FirstName == "Kodak").Id,
                PropertyId = propertyInDatabase.First(s => s.AgentId == UserInDatabase.First(s => s.FirstName == "Kelvin").Id).Id
               },
                new(){
                Id = 0,
                Date = DateTime.Now.AddDays(10),
                Amount = 1550000000,
                Status = OfferStatus.Pending,
                ClientId = UserInDatabase.First(s => s.FirstName == "Kodak").Id,
                PropertyId = propertyInDatabase.First(s => s.AgentId == UserInDatabase.First(s => s.FirstName == "Kelvin").Id).Id
               },
                new(){
                Id = 0,
                Date = DateTime.Now.AddDays(20),
                Amount = 123000000,
                Status = OfferStatus.Pending,
                ClientId = UserInDatabase.First(s => s.FirstName == "Kodak").Id,
                PropertyId = propertyInDatabase.First(s => s.AgentId == UserInDatabase.First(s => s.FirstName == "Kelvin").Id).Id
               }

            };


            //act
            foreach (var offer in offers)
            {
                var saveOffers = await service.AddAsync(offer);
            }

            var result = await service.GetAllList();


            //Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().HaveCountGreaterThan(1);
        }





        [Fact]
        public async Task GetAllLIst_Should_Return_Empty_When_Not_Exists()
        {


            //Arrange
            var service = CreateService();

            //act

            var result = await service.GetAllList();


            //Assert
            result.Should().BeEmpty();
            result.Should().HaveCount(0);
        }





        [Fact]
        public async Task GetListByIdClientAndPropertyIdAsync_Should_Return_Offers_When_Exists()
        {

            //Arrange
            var service = CreateService();

            (var PropertyInDatabase, var UserInDatabase) = await CreateProperty();

            var agent = UserInDatabase.First(s => s.FirstName == "Kelvin");
            var client = UserInDatabase.First(s => s.FirstName == "Kodak");

            var offers = new List<CreateOfferDto>
            {
               new(){
                Id = 0,
                Date = DateTime.Now,
                Amount = 1780000000,
                Status = OfferStatus.Pending,
                ClientId = client.Id,
                PropertyId = PropertyInDatabase.First(s => s.AgentId == agent.Id).Id
               },

                new(){
                Id = 0,
                Date = DateTime.Now.AddDays(10),
                Amount = 1550000000,
                Status = OfferStatus.Pending,
                ClientId = client.Id,
                PropertyId = PropertyInDatabase.First(s => s.AgentId == agent.Id).Id
               },

                new(){
                Id = 0,
                Date = DateTime.Now.AddDays(20),
                Amount = 123000000,
                Status = OfferStatus.Pending,
                ClientId = client.Id,
                PropertyId = PropertyInDatabase.First(s => s.AgentId == agent.Id).Id
               }

            };


            //act
            foreach (var offer in offers)
            {
                var saveOffers = await service.AddAsync(offer);
            }

            var result = await service.GetListByIdClientAndPropertyIdAsync(client.Id,PropertyInDatabase.First(s => s.AgentId == agent.Id).Id);


            //Assert
            result.Should().NotBeEmpty();
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().HaveCountGreaterThan(1);
        }






        [Fact]
        public async Task GetListByIdClientAndPropertyIdAsync_Should_Return_Empty_When_Not_Exists()
        {

            //Arrange
            var service = CreateService();


            //act
           
            var result = await service.GetListByIdClientAndPropertyIdAsync("4939ffkkdkd",1500);


            //Assert
            result.Should().BeEmpty();
         
        }




        [Fact]
        public async Task IsOfferActive_Should_Return_true_When_offer_Exists()
        {

            //Arrange
            var service = CreateService();

            (var propertyInDatabase, var userInDatabase) = await CreateProperty();


            var client = userInDatabase.First(s => s.FirstName == "Kodak");
            var agent = userInDatabase.First(s => s.FirstName == "Kelvin");
            var property = propertyInDatabase.First(s => s.AgentId == agent.Id);


            var offer = new CreateOfferDto
            {

                Id = 0,
                Date = DateTime.Now,
                Amount = 1780000000,
                Status = OfferStatus.Pending,
                ClientId = client.Id,
                PropertyId = property.Id

            };


            //act
            var saveOffer = await service.AddAsync(offer);
            var result = await service.IsOfferActive(client.Id,property.Id);

            //Assert
            result.Should().BeTrue();
         
        }



        [Fact]
        public async Task IsOfferActive_Should_Return_false_When_offer_Exists()
        {


            //Arrange
            var service = CreateService();

            (var propertyInDatabase, var userInDatabase) = await CreateProperty();


            var client = userInDatabase.First(s => s.FirstName == "Kodak");
            var agent = userInDatabase.First(s => s.FirstName == "Kelvin");
            var property = propertyInDatabase.First(s => s.AgentId == agent.Id);



            //act
            var result = await service.IsOfferActive(client.Id, property.Id);

            //Assert
            result.Should().BeFalse();

        }






    }
}
