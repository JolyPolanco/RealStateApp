using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Domain.Common.Enums;
using RealStateApp.Core.Domain.Entities;
using RealStateApp.Infraestructure.Identity.Contexts;
using RealStateApp.Infraestructure.Identity.Entities;
using RealStateApp.Infraestructure.Persistence.Contexts;
using RealStateApp.Infraestructure.Persistence.Repositories;

namespace RealStateApp.Integration.Tests.Persistence.Repositories
{
    public class MenssageRepositoryTests
    {


        private readonly DbContextOptions<RealStateContext> dbContextOptions;
        private readonly DbContextOptions<IdentityContext> IdentityDbContextOptions;



        public MenssageRepositoryTests()
        {

            dbContextOptions = new DbContextOptionsBuilder<RealStateContext>()
                .UseInMemoryDatabase(databaseName: $"DbTestMenssage_{Guid.NewGuid()}")
                .Options;



            IdentityDbContextOptions = new DbContextOptionsBuilder<IdentityContext>()
                .UseInMemoryDatabase(databaseName: $"IdentityDbTestMenssage_{Guid.NewGuid()}")
                .Options;

        }



        #region private method creacion de usuario
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


        #region Private method CrearPropiedad
        public async Task<(Property, List<AppUser>)> CreateProperty()
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
            var property = new Property()
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
            };

            var repo = new PropertyRepository(context);
            var SaveProperty = await repo.AddAsync(property);


            //asignamos mejoras a la propiedad

            var PropertyImprovement = new List<PropertyImprovement>
            {

                new() { ImprovementId = entitiesImprovement!.FirstOrDefault(s => s.Name == "Cocina")!.Id, PropertyId = SaveProperty.Id},
                new() { ImprovementId = entitiesImprovement!.FirstOrDefault(s => s.Name == "Baño")!.Id, PropertyId = SaveProperty.Id}

            };

            var PropertyImprovementRepo = new PropertyImprovementRepository(context);
            await PropertyImprovementRepo.AddRangeAsync(PropertyImprovement);
            return (SaveProperty, _UserInDatabase);
        }
        #endregion




        [Fact]
        public async Task AddAsync_Should_return_Menssage_when_Add_to_Database()
        {

            //Arrange
            var context = new RealStateContext(dbContextOptions);

            var repo = new MessageRepository(context);

            //Act


            (var PropertyInDatabase, var _userInDatabase) = await CreateProperty();
            var menssage = new Message()
            {

                Id = 0,
                Date = DateTime.Now,
                PropertyId = PropertyInDatabase.Id,
                Content = "Hola, Saludos principalmente muchos gusto, le envio este mensaje porque estoy interesado es esta propiedad",
                ReceiverUserId = _userInDatabase.FirstOrDefault(s => s.FirstName == "Kodak")!.Id,
                SenderUserId = _userInDatabase.FirstOrDefault(s => s.FirstName == "Kelvin")!.Id
            };

            var result = await repo.AddAsync(menssage);

            //Assert
            result.Should().NotBeNull();
            result.Content.Should().Be(menssage.Content);

        }




        [Fact]
        public async Task AddAsync_Should_return_Throw_when_Menssage_Is_Null()
        {

            //Arrange
            var context = new RealStateContext(dbContextOptions);

            var repo = new MessageRepository(context);


            //Act
            Func<Task> result = async () => await repo.AddAsync(null!);


            //Assert
            await result.Should().ThrowAsync<ArgumentNullException>();


        }




        [Fact]
        public async Task GetByIdAsync_Should_return_Menssage_when_exists()
        {

            //Arrange
            var context = new RealStateContext(dbContextOptions);

            var repo = new MessageRepository(context);

            //Act


            (var PropertyInDatabase, var _userInDatabase) = await CreateProperty();
            var menssage = new Message()
            {

                Id = 0,
                Date = DateTime.Now,
                PropertyId = PropertyInDatabase.Id,
                Content = "Hola, Saludos principalmente muchos gusto, le envio este mensaje porque estoy interesado es esta propiedad",
                ReceiverUserId = _userInDatabase.FirstOrDefault(s => s.FirstName == "Kodak")!.Id,
                SenderUserId = _userInDatabase.FirstOrDefault(s => s.FirstName == "Kelvin")!.Id
            };

            var SaveMenssage = await repo.AddAsync(menssage);
            var result = await repo.GetByIdAsync(SaveMenssage.Id);



            //Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Id.Should().Be(SaveMenssage.Id);
            result.Content.Should().Be(SaveMenssage.Content);

        }





        [Fact]
        public async Task GetByIdAsync_Should_return_null_when_not_exists()
        {
            //Arrange
            var context = new RealStateContext(dbContextOptions);

            var repo = new MessageRepository(context);

            //Act
            var result = await repo.GetByIdAsync(1500);


            //Assert
            result.Should().BeNull();

        }







        [Fact]
        public async Task DeleteAsync_Should_return_Null_when_exists()
        {


            //Arrange
            var context = new RealStateContext(dbContextOptions);

            var repo = new MessageRepository(context);

            //Act


            (var PropertyInDatabase, var _userInDatabase) = await CreateProperty();
            var menssage = new Message()
            {

                Id = 0,
                Date = DateTime.Now,
                PropertyId = PropertyInDatabase.Id,
                Content = "Hola, Saludos principalmente muchos gusto, le envio este mensaje porque estoy interesado es esta propiedad",
                ReceiverUserId = _userInDatabase.FirstOrDefault(s => s.FirstName == "Kodak")!.Id,
                SenderUserId = _userInDatabase.FirstOrDefault(s => s.FirstName == "Kelvin")!.Id
            };

            var SaveMenssage = await repo.AddAsync(menssage);
            await repo.DeleteAsync(SaveMenssage.PropertyId);
            var result = await repo.GetByIdAsync(SaveMenssage.Id);

            //Assert
            result.Should().BeNull();

        }





        [Fact]
        public async Task DeleteAsync_Should_return_No_Throw_when_No_found()
        {

            //Arrange
            //creamos usuarios
            var context = new RealStateContext(dbContextOptions);


            var MenssageRepo = new MessageRepository(context);

            Func<Task> act = async () => await MenssageRepo.GetByIdAsync(999);

            //Assert
            await act.Should().NotThrowAsync();

        }





        [Fact]
        public async Task UpdateAsync_Should_Modify_Message_InDatabase()
        {
            //Arrange
            var context = new RealStateContext(dbContextOptions);

            var repo = new MessageRepository(context);

            //Act


            (var PropertyInDatabase, var _userInDatabase) = await CreateProperty();
            var menssage = new Message()
            {

                Id = 0,
                Date = DateTime.Now,
                PropertyId = PropertyInDatabase.Id,
                Content = "Hola, Saludos principalmente muchos gusto, le envio este mensaje porque estoy interesado es esta propiedad",
                ReceiverUserId = _userInDatabase.FirstOrDefault(s => s.FirstName == "Kodak")!.Id,
                SenderUserId = _userInDatabase.FirstOrDefault(s => s.FirstName == "Kelvin")!.Id
            };

            var SaveMenssage = await repo.AddAsync(menssage);
            SaveMenssage.Content = "Holaaaaaaaa!";
            var result = await repo.UpdateAsync(SaveMenssage.Id, SaveMenssage);

            //Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(SaveMenssage.Id);
            result.Content.Should().Be(SaveMenssage.Content);


        }




        [Fact]
        public async Task UpdateAsync_Should_return_null_when_Menssage_NoFound()
        {
            var context = new RealStateContext(dbContextOptions);

            var repo = new MessageRepository(context);

            //Act


            (var PropertyInDatabase, var _userInDatabase) = await CreateProperty();
            var Fakemenssage = new Message()
            {

                Id = 0,
                Date = DateTime.Now,
                PropertyId = PropertyInDatabase.Id,
                Content = "Hola, Saludos principalmente muchos gusto, le envio este mensaje porque estoy interesado es esta propiedad",
                ReceiverUserId = _userInDatabase.FirstOrDefault(s => s.FirstName == "Kodak")!.Id,
                SenderUserId = _userInDatabase.FirstOrDefault(s => s.FirstName == "Kelvin")!.Id
            };

            var result = await repo.UpdateAsync(Fakemenssage.Id, Fakemenssage);

            //Assert
            result.Should().BeNull();

        }





        [Fact]
        public async Task GetAllAsync_Should_Return_All_Menssage()
        {

            var context = new RealStateContext(dbContextOptions);

            var repo = new MessageRepository(context);

            //Act


            (var PropertyInDatabase, var _userInDatabase) = await CreateProperty();
            var menssages = new List<Message>
            {

               new(){ Id = 0,
                Date = DateTime.Now,
                PropertyId = PropertyInDatabase.Id,
                Content = "Hola, Saludos principalmente muchos gusto, le envio este mensaje porque estoy interesado es esta propiedad",
                ReceiverUserId = _userInDatabase.FirstOrDefault(s => s.FirstName == "Kodak")!.Id,
                SenderUserId = _userInDatabase.FirstOrDefault(s => s.FirstName == "Kelvin")!.Id
               },
               new(){ Id = 0,
                Date = DateTime.Now.AddDays(20),
                PropertyId = PropertyInDatabase.Id,
                Content = "Holaaaaaa!, que tal",
                ReceiverUserId = _userInDatabase.FirstOrDefault(s => s.FirstName == "Kodak")!.Id,
                SenderUserId = _userInDatabase.FirstOrDefault(s => s.FirstName == "Kelvin")!.Id
               },
                new(){ Id = 0,
                Date = DateTime.Now.AddDays(10),
                PropertyId = PropertyInDatabase.Id,
                Content = "Holaaaaaa!, que tal",
                ReceiverUserId = _userInDatabase.FirstOrDefault(s => s.FirstName == "Kodak")!.Id,
                SenderUserId = _userInDatabase.FirstOrDefault(s => s.FirstName == "Kelvin")!.Id
               },
            };

            await repo.AddRangeAsync(menssages);
            var result = await repo.GetAllList();


            //Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().HaveCountGreaterThan(1);

        }



        [Fact]
        public async Task GetAllAsync_Should_Empty_when_Menssage_Not_Exits()
        {

            //Arrange
            //creamos usuarios

            var context = new RealStateContext(dbContextOptions);

            //Act

            var menssageRepo = new MessageRepository(context);
            var result = await menssageRepo.GetAllList();


            //Assert
            result.Should().HaveCount(0);
            result.Should().BeEmpty();
        }




    }
}
