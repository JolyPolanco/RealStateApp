
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using RealStateApp.Core.Application.Dtos.User;
using RealStateApp.Core.Application.Interfaces;
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
    public class HomeClientServiceTests
    {



        private readonly DbContextOptions<RealStateContext> dbContextOptions;
        private readonly DbContextOptions<IdentityContext> IdentitydbContextOptions;
        private readonly Mock<IAccountServiceForWebApp> accountServiceForWebAppMock;
        private readonly Mock<IUserService> userServiceMock;

        private readonly IMapper _mapper;




        public HomeClientServiceTests()
        {


            dbContextOptions = new DbContextOptionsBuilder<RealStateContext>()
             .UseInMemoryDatabase(databaseName: $"TestsHomeServicedb_{Guid.NewGuid()}")
             .Options;


            IdentitydbContextOptions = new DbContextOptionsBuilder<IdentityContext>()
             .UseInMemoryDatabase(databaseName: $"IdentityTestsHomeServiceDB_{Guid.NewGuid()}")
             .Options;


            accountServiceForWebAppMock = new Mock<IAccountServiceForWebApp>();
            userServiceMock = new Mock<IUserService>();


            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });


            var config = new MapperConfiguration(cf =>
            {

                cf.AddProfile<UserDtosAndViewModelMappingProfile>();
                cf.AddProfile<PropertyToViewModelMappingProfile>();
                cf.AddProfile<PropertyToDtoMappingProfile>();


            }, loggerFactory);


            _mapper = config.CreateMapper();

        }



        private HomeClientService CreateService()
        {

            var Contexto = new RealStateContext(dbContextOptions);
            var PropertyRepo = new PropertyRepository(Contexto);
            var PropertyTypeRepository = new PropertyTypeRepository(Contexto);
            var SaleTypeRepository = new SaleTypeRepository(Contexto);
            var ImprovemnteRepo = new ImprovementRepository(Contexto);
            var PropertyphotoRepo = new PropertyPhotoRepository(Contexto);
            var PropertyImprovementRepo = new PropertyImprovementRepository(Contexto);
            var FavoritePropertyRepo = new FavoritePropertyRepoitory(Contexto);


            var service = new HomeClientService(_mapper, PropertyRepo, PropertyTypeRepository, SaleTypeRepository, PropertyphotoRepo, accountServiceForWebAppMock.Object, ImprovemnteRepo, PropertyImprovementRepo, FavoritePropertyRepo, userServiceMock.Object);
            return service;

        }



        #region private method creacion de usuario
        private async Task<List<AppUser>> CreateUsers()
        {
            var passwordHasher = new PasswordHasher<AppUser>();
            var Identitycontext = new IdentityContext(IdentitydbContextOptions);

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



        #region LitsUserAgent
        private async Task<List<AppUser>> CreateUsersAgent()
        {
            var passwordHasher = new PasswordHasher<AppUser>();
            var Identitycontext = new IdentityContext(IdentitydbContextOptions);

            var users = new List<AppUser>
            {

                new() { Id = Guid.NewGuid().ToString(), FirstName = "Agent1", LastName = "KIOAgent", Email = "KIO@gmail.com", EmailConfirmed = true, IsActive = true, PhoneNumber = "584994003", UserName = "KMLJS"},
                new() { Id = Guid.NewGuid().ToString(), FirstName = "AgentThree", LastName = "AgentDiKey", Email = "Key@gmail.com", EmailConfirmed = true, IsActive = true, PhoneNumber = "0492000003", UserName = "NLE_Chopa", },

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


                new() { RoleId = rolesInDatabase.FirstOrDefault(s => s.Name == "AGENT" )!.Id, UserId = _UserInDatabase.FirstOrDefault(s => s.FirstName == "Agent1")!.Id},
                new() { RoleId = rolesInDatabase.FirstOrDefault(s => s.Name == "AGENT")!.Id, UserId = _UserInDatabase.FirstOrDefault(s => s.FirstName == "AgentThree")!.Id},
            };


            await Identitycontext.UserRoles.AddRangeAsync(userRoles);
            await Identitycontext.SaveChangesAsync();
            var userRolesInDatabase = await Identitycontext.UserRoles.ToListAsync();
            return _UserInDatabase;

        }
       #endregion


        #region Private method CrearPropieda
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
        public async Task ListPropertyAsync_Should_Return_All_Properties_when_Available()
        {


            //Arrange
            var service = CreateService();

            (var propertiesInDatabase, var userInDatabase) = await CreateProperty();


            //Act
             var result = await service.ListProperty(userInDatabase.FirstOrDefault(s => s.FirstName == "Kodak")!.Id);



            //Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().HaveCountGreaterThan(1);
            result.Should().NotBeEmpty();
        }



        [Fact]
        public async Task ListPropertyAsync_Should_Return_Empty_when_Not_Properties()
        {


            //Arrange
            var service = CreateService();

            //Act
            var result = await service.ListProperty("kfkfklslsoeoeos");


            //Assert
            result.Should().BeEmpty();
            result.Should().HaveCount(0);
        
           
        }




        [Fact]
        public async Task ListDetailProperty_Should_Return_DetailsProperty_When_Property_Exists()
        {

            var service = CreateService();
            (var propertiesInDatabase, var userInDatabase) = await CreateProperty();

            var agent = userInDatabase.First(u => u.FirstName == "Kelvin");

            accountServiceForWebAppMock
                .Setup(s => s.GetById(agent.Id))
                .ReturnsAsync(new UserDto
                {
                    Id = agent.Id,
                    FirstName = agent.FirstName,
                    Email = agent.Email!,
                    Photo = agent.Photo,
                    PhoneNumber = agent.PhoneNumber,
                    Dni = "",
                    LastName = agent.LastName,
                    Role = "",
                    UserName = "NIOKEY",
                });

            var property = propertiesInDatabase.First(p => p.AgentId == agent.Id);


            // Act
            var result = await service.ListDetailProperty(property.Id);

            // Assert
            result.Should().NotBeNull();
            result.AgentId.Should().Be(agent.Id);



        }






        [Fact]
        public async Task FilterByCode_Should_Return_DataProperty_When_Property_Exists()
        {
            //Arrange
            var service = CreateService();

            (var propertiesInDatabase,var userInDatabase) = await CreateProperty();


            var client = userInDatabase.First(s => s.FirstName == "Kodak");
            var agent = userInDatabase.First(s => s.FirstName == "Kelvin");


            accountServiceForWebAppMock
                .Setup(s => s.GetById(client.Id))
                .ReturnsAsync(new UserDto()
                {
                    Id = client.Id,
                    FirstName = client.FirstName,
                    Email = client.Email!,
                    Photo = client.Photo,
                    PhoneNumber = client.PhoneNumber,
                    Dni = "",
                    IsActive = client.IsActive,
                    LastName = client.LastName,
                    UserName = "",
                    Role = ""

                });

            var property = propertiesInDatabase.First(s => s.AgentId == agent.Id);


            // Act
            var result = await service.FilterByCode(property.Code,client.Id);

            // Assert
            result.Should().NotBeNull();
            result.Code.Should().Be(property.Code);
            result.AgentId.Should().Be(property.AgentId);
     
        }







        [Fact]
        public async Task FilterByCode_Should_Return_Null_When_Property_Not_Exists()
        {

            //Arrange
            var service = CreateService();


            (var propertiesInDatabase, var userInDatabase) = await CreateProperty();



            // Act
            var result = await service.FilterByCode("roodosmep", "fififififfo");

             
            // Assert
            result.Should().BeNull();
           
        }




        [Fact]
        public async Task FilterMultiple_Should_Return_DataProperty_When_PropertyType_Exist()
        {

            //Arrange
            var service = CreateService();
            var context = new RealStateContext(dbContextOptions);
            var PropertyTypeRepo = new PropertyTypeRepository(context);

            (var propertiesInDatabase, var userInDatabase) = await CreateProperty();


            var property = propertiesInDatabase.First(s => s.Code == "MAP-3920");
            var propertyType = await PropertyTypeRepo.GetByIdAsync(property.PropertyTypeId);
            

            // Act
            var result = await service.FilterMultiple(propertyType!.Name, null, null, null, null, null);

             
            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Should().HaveCount(3);   
            result.Should().HaveCountGreaterThan(1);   
      
        }



        [Fact]
        public async Task FilterMultiple_Should_Return_DataProperty_When_PropertyType_and_PriceMinAndMaxExist()
        {

            //Arrange
            var service = CreateService();
            var context = new RealStateContext(dbContextOptions);
            var PropertyTypeRepo = new PropertyTypeRepository(context);

            (var propertiesInDatabase, var userInDatabase) = await CreateProperty();


            var property = propertiesInDatabase.First(s => s.Code == "MAP-3920");
            var propertyType = await PropertyTypeRepo.GetByIdAsync(property.PropertyTypeId);


            // Act
            var result = await service.FilterMultiple(propertyType!.Name, 400000, 1100000000, null, null, null);


            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Should().HaveCount(2);
            result.Should().HaveCountGreaterThan(1);

        }



        [Fact]
        public async Task FilterMultiple_Should_Return_DataProperty_When_Exist()
        {

            //Arrange
            var service = CreateService();
            var context = new RealStateContext(dbContextOptions);
            var PropertyTypeRepo = new PropertyTypeRepository(context);

            (var propertiesInDatabase, var userInDatabase) = await CreateProperty();


            var property = propertiesInDatabase.First(s => s.Code == "MAP-3920");
            var propertyType = await PropertyTypeRepo.GetByIdAsync(property.PropertyTypeId);


            // Act
            var result = await service.FilterMultiple(propertyType!.Name, 400000, 1100000000, 10 , 5 , null);


            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Should().HaveCount(1);
            result.Should().HaveCountGreaterThan(0);

        }





        [Fact]
        public async Task ListAgentAsync_should_return_Agents_When_Exists()
        {
        
            //arrange
            var service = CreateService();

            var agents = await CreateUsersAgent();

            userServiceMock
                .Setup(s => s.GetUsersAgentnOnly())
                .ReturnsAsync(agents.Select(s => new UserDto() {

                    Id = s.Id,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Dni = s.Dni ?? "",
                    Email = s.Email ?? "",
                    IsActive = s.IsActive,
                    IsVerified = s.EmailConfirmed,
                    PhoneNumber = s.PhoneNumber,
                    Photo = s.Photo,
                    Role = "",
                    UserName = s.UserName ?? ""

                }).ToList());


            //act
            var result =  await service.ListAgentAsync();


            //assert
            result.Should().NotBeEmpty();
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().HaveCountGreaterThan(0);

        }





        [Fact]
        public async Task ListPropertyAgentAsync_should_return_Properties_When_Exists()
        {

            //arrange
            var service = CreateService();

            (var PropertiesInDatabase, var UserInDatabase) = await CreateProperty();

            var agent = UserInDatabase.First(s => s.FirstName == "Kelvin");
            var client = UserInDatabase.First(s => s.FirstName == "Kodak");


  
            //act
            var result = await service.ListPropertyAgentAsync(agent.Id,client.Id);


            //assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(3);
            result.Should().HaveCountGreaterThan(1);  
        }


        [Fact]
        public async Task ListPropertyAgentAsync_should_return_Empty_When_Not_Exists()
        {

            //arrange
            var service = CreateService();

          
            //act
            var result = await service.ListPropertyAgentAsync("ieieeooe", "client.Id");


            //assert
            result.Should().BeNullOrEmpty();
            result.Should().HaveCount(0);
        }



        [Fact]
        public async Task GetAgentByName_should_return_Agents_OR_Agent_When_Exists_()
        {

            //arrange
            var service = CreateService();

            var agents = await  CreateUsersAgent();

            var agent = agents.FirstOrDefault(s => s.FirstName == "AgentThree");

            accountServiceForWebAppMock
                .Setup(s => s.GetByNameAsync(agent!.FirstName))
                .ReturnsAsync(agents.Select(s => new UserDto()
                {

                    Id = s!.Id,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    UserName = s.UserName ?? "",
                    Role = AppRoles.AGENT.ToString(),
                    Email = s.Email ?? "",
                    Dni = s.Dni ?? "",
                    IsActive = s.IsActive,
                    IsVerified = s.EmailConfirmed,
                    PhoneNumber = s.PhoneNumber ?? "",
                    Photo = s.Photo ?? "",

                }).ToList());
               
            //act
            var result = await service.GetAgentByName(agent!.FirstName);


            //assert
            result.Should().NotBeEmpty();
            result.Should().HaveCount(2);
            result.Should().HaveCountGreaterThan(0);
        }




        [Fact]
        public async Task GetAgentByName_should_return_Empty_When_Not_Exists()
        {

            //arrange
            var service = CreateService();


         
            //act
            var result = await service.GetAgentByName("ZZZZZ");


            //assert
            result.Should().BeEmpty();
            result.Should().HaveCount(0);
        }

    }








}






