using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealStateApp.Core.Application.Dtos.Message;
using RealStateApp.Core.Application.Mappings.DtosAndViewModels;
using RealStateApp.Core.Application.Services;
using RealStateApp.Core.Domain.Common.Enums;
using RealStateApp.Core.Domain.Entities;
using RealStateApp.Infraestructure.Identity.Contexts;
using RealStateApp.Infraestructure.Identity.Entities;
using RealStateApp.Infraestructure.Persistence.Contexts;
using RealStateApp.Infraestructure.Persistence.Repositories;


namespace RealStateApp.Unit.Tests.Services
{
    public class MenssageServiceTests
    {

        private readonly DbContextOptions<RealStateContext> dbContextOptions;
        private readonly DbContextOptions<IdentityContext> IdentityDbContextOptions;
        private readonly IMapper Mapper;



        public MenssageServiceTests()
        {

            dbContextOptions = new DbContextOptionsBuilder<RealStateContext>()
                .UseInMemoryDatabase(databaseName: $"DbTestMenssageSevice_{Guid.NewGuid()}")
                .Options;



            IdentityDbContextOptions = new DbContextOptionsBuilder<IdentityContext>()
                .UseInMemoryDatabase(databaseName: $"IdentityDbCTestMenssage_{Guid.NewGuid()}")
                .Options;



            var loggerFactory = LoggerFactory.Create(cf =>
            {

                cf.AddConsole();
            });



            var config = new MapperConfiguration(cf =>
            {

                cf.AddProfile<MessageAndDtoMappingProfile>();

            }, loggerFactory);



            Mapper = config.CreateMapper();

        }




        private MessageService CreateService()
        {
            var context = new RealStateContext(dbContextOptions);

            var repo = new MessageRepository(context);


            var service = new MessageService(repo, Mapper);
            return service;

        }

        #region private methods Create User
        private async Task<List<AppUser>> CreateUsers()
        {
            var passwordHasher = new PasswordHasher<AppUser>();
            var Identitycontext = new IdentityContext(IdentityDbContextOptions);

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
        public async Task AddAsync_Should_return_Dto_when_Added()
        {


            //Arrange
            var service = CreateService();

            (var propertyInDatabase, var UserInDatabase) = await CreateProperty();

            var Client = UserInDatabase.First(s => s.FirstName == "Kodak");
            var Agent = UserInDatabase.First(s => s.FirstName == "Kelvin");
            var property = propertyInDatabase.First(s => s.AgentId == Agent.Id);

            var Menssage = new CreateMessageDto()
            {

                Id = 0,
                PropertyId = property.Id,
                Date = DateTime.Now,
                Content = "Hola, principalmente saludos mucho gusto le envio este mensaje,porque estoy interesado en esta propiedad...",
                SenderUserId = Client.Id,
                ReceiverUserId = Agent.Id,
            };

            //Act
            var result = await service.AddAsync(Menssage);



            //Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Content.Should().Be(Menssage.Content);

        }





        [Fact]
        public async Task AddAsync_Should_return_null_when_Not_Added()
        {


            //Arrange
            var service = CreateService();


            //Act
            var result = await service.AddAsync(null!);



            //Assert
            result.Should().BeNull();


        }




        [Fact]
        public async Task GetByIdAsync_Should_return_Dto_when_Exists()
        {


            //Arrange
            var service = CreateService();

            (var propertyInDatabase, var UserInDatabase) = await CreateProperty();

            var Client = UserInDatabase.First(s => s.FirstName == "Kodak");
            var Agent = UserInDatabase.First(s => s.FirstName == "Kelvin");
            var property = propertyInDatabase.First(s => s.AgentId == Agent.Id);

            var Menssage = new CreateMessageDto()
            {

                Id = 0,
                PropertyId = property.Id,
                Date = DateTime.Now,
                Content = "Hola, principalmente saludos mucho gusto le envio este mensaje,porque estoy interesado en esta propiedad...",
                SenderUserId = Client.Id,
                ReceiverUserId = Agent.Id,
            };

            //Act
            var SaveMenssage = await service.AddAsync(Menssage);
            var result = await service.GetByIdAsync(SaveMenssage!.Id);


            //Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Id.Should().Be(SaveMenssage.Id);
            result.Content.Should().Be(SaveMenssage.Content);

        }




        [Fact]
        public async Task GetByIdAsync_Should__return_null_Dto_when_Not_Exists()
        {


            //Arrange
            var service = CreateService();


            //Act

            var result = await service.GetByIdAsync(999);


            //Assert
            result.Should().BeNull();

        }





        [Fact]
        public async Task DeleteAsync_Should_return_null_Dto_when__Exists()
        {



            //Arrange
            var service = CreateService();

            (var propertyInDatabase, var UserInDatabase) = await CreateProperty();

            var Client = UserInDatabase.First(s => s.FirstName == "Kodak");
            var Agent = UserInDatabase.First(s => s.FirstName == "Kelvin");
            var property = propertyInDatabase.First(s => s.AgentId == Agent.Id);

            var Menssage = new CreateMessageDto()
            {

                Id = 0,
                PropertyId = property.Id,
                Date = DateTime.Now,
                Content = "Hola, principalmente saludos mucho gusto le envio este mensaje,porque estoy interesado en esta propiedad...",
                SenderUserId = Client.Id,
                ReceiverUserId = Agent.Id,
            };

            //Act
            var SaveMenssage = await service.AddAsync(Menssage);
            await service.DeleteAsync(SaveMenssage!.Id);
            var result = await service.GetByIdAsync(SaveMenssage.Id);


            //Assert
            result.Should().BeNull();


        }





        [Fact]
        public async Task DeleteAsync_Should_return_Throw_when_Dto_no_exists()
        {



            //Arrange
            var service = CreateService();


            //Act

            Func<Task> act = async () => await service.DeleteAsync(999);


            //Assert
            await act.Should().ThrowAsync<Exception>();


        }




        [Fact]
        public async Task UpdateAsync_Should_return_Menssage_when_Dto_Updated()
        {



            //Arrange
            var service = CreateService();


            (var propertyInDatabase, var UserInDatabase) = await CreateProperty();

            var Client = UserInDatabase.First(s => s.FirstName == "Kodak");
            var Agent = UserInDatabase.First(s => s.FirstName == "Kelvin");
            var property = propertyInDatabase.First(s => s.AgentId == Agent.Id);

            var Menssage = new CreateMessageDto()
            {

                Id = 0,
                PropertyId = property.Id,
                Date = DateTime.Now,
                Content = "Hola, principalmente saludos mucho gusto le envio este mensaje,porque estoy interesado en esta propiedad...",
                SenderUserId = Client.Id,
                ReceiverUserId = Agent.Id,
            };

            //Act
            var SaveMenssage = await service.AddAsync(Menssage);
            SaveMenssage!.Content = "Holaaaaaa!";
            var result = await service.UpdateAsync(SaveMenssage.Id, SaveMenssage);

            //Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(result.Id);
            result.Content.Should().Be(result.Content);


        }




        [Fact]
        public async Task UpdateAsync_Should_return_null_when_Dto_Not_Updated()
        {



            //Arrange
            var service = CreateService();


            (var propertyInDatabase, var UserInDatabase) = await CreateProperty();

            var Client = UserInDatabase.First(s => s.FirstName == "Kodak");
            var Agent = UserInDatabase.First(s => s.FirstName == "Kelvin");
            var property = propertyInDatabase.First(s => s.AgentId == Agent.Id);

            var Menssage = new CreateMessageDto()
            {

                Id = 0,
                PropertyId = property.Id,
                Date = DateTime.Now,
                Content = "Hola, principalmente saludos mucho gusto le envio este mensaje,porque estoy interesado en esta propiedad...",
                SenderUserId = Client.Id,
                ReceiverUserId = Agent.Id,
            };

            //Act
            var result = await service.UpdateAsync(Menssage.Id, Menssage);

            //Assert
            result.Should().BeNull();



        }


        [Fact]
        public async Task GetAllList_Should_return_all_Menssage_when_Exists()
        {



            //Arrange
            var service = CreateService();


            (var propertyInDatabase, var UserInDatabase) = await CreateProperty();

            var Client = UserInDatabase.First(s => s.FirstName == "Kodak");
            var Agent = UserInDatabase.First(s => s.FirstName == "Kelvin");
            var property = propertyInDatabase.First(s => s.AgentId == Agent.Id);

            var Menssages = new List<CreateMessageDto>
            {

                new()
                {
                Id = 0,
                PropertyId = property.Id,
                Date = DateTime.Now.AddDays(10),
                Content = "Hola, principalmente saludos mucho gusto le envio este mensaje,porque estoy interesado en esta propiedad...",
                SenderUserId = Client.Id,
                ReceiverUserId = Agent.Id
                },
                 new()
                {
                Id = 0,
                PropertyId = property.Id,
                Date = DateTime.Now.AddDays(12),
                Content = "Hola, Que tal???",
                SenderUserId = Client.Id,
                ReceiverUserId = Agent.Id
                },
                  new()
                {
                Id = 0,
                PropertyId = property.Id,
                Date = DateTime.Now.AddDays(15),
                Content = "Holaaaaaaaaaa!",
                SenderUserId = Client.Id,
                ReceiverUserId = Agent.Id
                }
            };



            //Act
            foreach (var item in Menssages)
            {
               await service.AddAsync(item);
            }

            var result = await service.GetAllList();
           
            //Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(3);
            result.Should().HaveCount(3);
            result.Should().HaveCountGreaterThan(1);


        }




        [Fact]
        public async Task GetAllList_Should_return_Empty_when_Not_Exists()
        {

            //Arrange
            var service = CreateService();


            //Act
           
            var result = await service.GetAllList();

            //Assert
            result.Should().BeEmpty();
            result.Should().HaveCount(0);
        }



        [Fact]
        public async Task GetConversaction_Should_return_All_Conversaction_not_null()
        {



            //Arrange
            var service = CreateService();


            (var propertyInDatabase, var UserInDatabase) = await CreateProperty();

            var Client = UserInDatabase.First(s => s.FirstName == "Kodak");
            var Agent = UserInDatabase.First(s => s.FirstName == "Kelvin");
            var property = propertyInDatabase.First(s => s.AgentId == Agent.Id);

            var Menssages = new List<CreateMessageDto>
            {

                new()
                {
                Id = 0,
                PropertyId = property.Id,
                Date = DateTime.Now.AddDays(10),
                Content = "Hola, principalmente saludos mucho gusto le envio este mensaje,porque estoy interesado en esta propiedad...",
                SenderUserId = Client.Id,
                ReceiverUserId = Agent.Id
                },
                 new()
                {
                Id = 0,
                PropertyId = property.Id,
                Date = DateTime.Now.AddDays(12),
                Content = "Hola, que tal la propiedad se encuentra disponible al precio  mostrado en el perfil, dicho precio es negociable si quieres puedo darte mas informacion de la ubicacion o cuentame que quieres saber en especifico!! ",
                SenderUserId = Agent.Id,
                ReceiverUserId = Client.Id
                },
                  new()
                {
                Id = 0,
                PropertyId = property.Id,
                Date = DateTime.Now.AddDays(15),
                Content = "Hola, perfecto me gustaria coordianar un dia para ver en fisico en local!!!",
                SenderUserId = Client.Id,
                ReceiverUserId = Agent.Id
                }
            };



            //Act
            foreach (var item in Menssages)
            {
                await service.AddAsync(item);
            }

            var result = await service.GetConversaction(property.Id,Client.Id,Agent.Id);



            //Assert
            result.Should().NotBeEmpty();
            result.Should().NotBeNull();
            result.Should().HaveCountGreaterThan(1);

        }


        [Fact]
        public async Task GetConversaction_Should_return_Empty_When_Conversaction_not_Exists()
        {



            //Arrange
            var service = CreateService();


            (var propertyInDatabase, var UserInDatabase) = await CreateProperty();

            var Client = UserInDatabase.First(s => s.FirstName == "Kodak");
            var Agent = UserInDatabase.First(s => s.FirstName == "Kelvin");
            var property = propertyInDatabase.First(s => s.AgentId == Agent.Id);



            //Act
            var result = await service.GetConversaction(property.Id, Client.Id, Agent.Id);



            //Assert
            result.Should().BeEmpty();
        }







    }
}
