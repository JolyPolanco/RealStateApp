using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Domain.Common.Enums;
using RealStateApp.Core.Domain.Entities;
using RealStateApp.Infraestructure.Identity.Contexts;
using RealStateApp.Infraestructure.Identity.Entities;
using RealStateApp.Infraestructure.Persistence.Contexts;
using RealStateApp.Infraestructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Integration.Tests.Persistence.Repositories
{
    public class FavoritePropertyRepositoryTest
    {


        private readonly DbContextOptions<RealStateContext> dbContextOptions;
        private readonly DbContextOptions<IdentityContext> identityContextOptions;



        public FavoritePropertyRepositoryTest()
        {


            dbContextOptions = new DbContextOptionsBuilder<RealStateContext>()
                .UseInMemoryDatabase(databaseName: $"DBcontextTestFavoriteProperty_{Guid.NewGuid()}")
                .Options;



            identityContextOptions = new DbContextOptionsBuilder<IdentityContext>()
                .UseInMemoryDatabase(databaseName: $"dbIdentityTestFavoriteProperty_{Guid.NewGuid()}")
                .Options;

        }



        #region private method creacion de usuario
        private async Task<List<AppUser>> CreateUsers()
        {
            var passwordHasher = new PasswordHasher<AppUser>();
            var Identitycontext = new IdentityContext(identityContextOptions);

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
        public async Task AddAsync_should_return_favariteProperty_when_Add_To_Database()
        {


            //arrange
            var context = new RealStateContext(dbContextOptions);
            var repo = new FavoritePropertyRepoitory(context);


            (var propertyInDatabase, var _userInDatabase) = await CreateProperty();



            var favoriteProperty = new FavoriteProperty()
            {


                Id = 0,
                ClientId = _userInDatabase.FirstOrDefault(s => s.FirstName == "Kodak")!.Id,
                PropertyId = propertyInDatabase.Id,

            };


            //Act
            var result = await repo.AddAsync(favoriteProperty);


            //Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);

        }




        [Fact]
        public async Task AddAsync_should_return_Throw_when_Add_Not_Add_To_Database()
        {


            //arrange
            var context = new RealStateContext(dbContextOptions);
            var repo = new FavoritePropertyRepoitory(context);



            //Act
            Func<Task> result = async () => await repo.AddAsync(null!);


            //Assert
            await result.Should().ThrowAsync<ArgumentNullException>();

        }




        [Fact]
        public async Task GetByIdAsync_Should_return_FavoriteProperty_when_exists()
        {


            //arrange
            var context = new RealStateContext(dbContextOptions);
            var repo = new FavoritePropertyRepoitory(context);


            (var propertyInDatabase, var _userInDatabase) = await CreateProperty();


            var favoriteProperty = new FavoriteProperty()
            {

                Id = 0,
                ClientId = _userInDatabase.FirstOrDefault(s => s.FirstName == "Kodak")!.Id,
                PropertyId = propertyInDatabase.Id,
            };



            //Act
            var SaveFavoriteProperty = await repo.AddAsync(favoriteProperty);
            var result = await repo.GetByIdAsync(SaveFavoriteProperty.Id);


            //Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Id.Should().Be(SaveFavoriteProperty.Id);

        }





        [Fact]
        public async Task GetByIdAsync_Should_return_null_when_not_exists()
        {
            //Arrange
            var context = new RealStateContext(dbContextOptions);

            var repo = new FavoritePropertyRepoitory(context);

            //Act
            var result = await repo.GetByIdAsync(1500);


            //Assert
            result.Should().BeNull();

        }







        [Fact]
        public async Task DeleteAsync_Should_return_Null_when_FavoritePropertyexists()
        {


            //arrange
            var context = new RealStateContext(dbContextOptions);
            var repo = new FavoritePropertyRepoitory(context);


            (var propertyInDatabase, var _userInDatabase) = await CreateProperty();



            var favoriteProperty = new FavoriteProperty()
            {

                Id = 0,
                ClientId = _userInDatabase.FirstOrDefault(s => s.FirstName == "Kodak")!.Id,
                PropertyId = propertyInDatabase.Id,

            };


            //Act
            var savefavoriteProperty = await repo.AddAsync(favoriteProperty);
            await repo.DeleteAsync(savefavoriteProperty.Id);
            var result = await repo.GetByIdAsync(savefavoriteProperty.Id);

            //Assert
            result.Should().BeNull();

        }





        [Fact]
        public async Task DeleteAsync_Should_return_No_Throw_when_FavoriteProperty_No_found()
        {

            //Arrange
            //creamos usuarios
            var context = new RealStateContext(dbContextOptions);


            var FavoritePropertyRepo = new FavoritePropertyRepoitory(context);

            Func<Task> act = async () => await FavoritePropertyRepo.GetByIdAsync(999);


            //Assert
            await act.Should().NotThrowAsync();

        }








        [Fact]
        public async Task UpdateAsync_Should_Modify_FavoriteProperty_InDatabase()
        {


            //arrange
            var context = new RealStateContext(dbContextOptions);
            var repo = new FavoritePropertyRepoitory(context);
            var propertyRepo = new PropertyRepository(context);


            (var propertyInDatabase, var _userInDatabase) = await CreateProperty();



            //creamos una propiedad para actualizar dicho id despue de agregar
            var property = new Property()
            {

                Id = 0,
                AgentId = _userInDatabase.FirstOrDefault(s => s.FirstName == "Kelvin")!.Id,
                Description = "Es una propiedad....",
                Bathrooms = 2,
                Bedrooms = 4,
                Code = "3030dkdps",
                Price = 1200000000,
                SizeInMeters = 200.0,
                Status = PropertyStatus.Available,
                PropertyTypeId = propertyInDatabase.PropertyTypeId,
                SaleTypeId = propertyInDatabase.SaleTypeId,

            };


            var SaveProperty = propertyRepo.AddAsync(property);


            var favoriteProperty = new FavoriteProperty()
            {
                Id = 0,
                ClientId = _userInDatabase.FirstOrDefault(s => s.FirstName == "Kodak")!.Id,
                PropertyId = propertyInDatabase.Id

            };


            //Act
            var SaveFavoriteProperty = await repo.AddAsync(favoriteProperty);
            SaveFavoriteProperty.PropertyId = SaveProperty.Id;
            var result = await repo.UpdateAsync(SaveProperty.Id, SaveFavoriteProperty);


            //Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.PropertyId.Should().Be(SaveProperty.Id);

        }






        [Fact]
        public async Task UpdateAsync_Should_return_null_when_FavoriteProperty_NoFound()
        {

            //arrange
            var context = new RealStateContext(dbContextOptions);

            var repo = new FavoritePropertyRepoitory(context);

            //Act

            (var PropertyInDatabase, var _userInDatabase) = await CreateProperty();
            var FakeFavoriteProperty = new FavoriteProperty()
            {

                Id = 0,
                PropertyId = 0,
                ClientId = "",
            };

            var result = await repo.UpdateAsync(FakeFavoriteProperty.Id, FakeFavoriteProperty);



            //Assert
            result.Should().BeNull();


        }







        [Fact]
        public async Task GetAllAsync_Should_Return_All_FavoriteProperty()
        {

            var context = new RealStateContext(dbContextOptions);

            var repo = new FavoritePropertyRepoitory(context);
            //Act


            (var PropertyInDatabase, var _userInDatabase) = await CreateProperty();
            var Favorites = new List<FavoriteProperty>
            {

               new(){
                Id = 0,
                PropertyId = PropertyInDatabase.Id,
                ClientId = _userInDatabase.FirstOrDefault(s => s.FirstName == "Kodak")!.Id

               },
               new(){
                 Id = 0,
                 PropertyId = PropertyInDatabase.Id,
                 ClientId   = _userInDatabase.FirstOrDefault(s => s.FirstName == "Kodak")!.Id,
               },
                new(){
                Id = 0,
                PropertyId = PropertyInDatabase.Id,
                 ClientId = _userInDatabase.FirstOrDefault(s => s.FirstName == "Kodak") !.Id

               },
            };

            await repo.AddRangeAsync(Favorites);
            var result = await repo.GetAllList();


            //Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().HaveCountGreaterThan(1);

        }



        [Fact]
        public async Task GetAllAsync_Should_Empty_when_FavoriteProperty_Not_Exits()
        {

            //Arrange
            //creamos usuarios

            var context = new RealStateContext(dbContextOptions);

            //Act

            var Favorite = new FavoritePropertyRepoitory(context);
            var result = await Favorite.GetAllList();


            //Assert
            result.Should().HaveCount(0);
            result.Should().BeEmpty();
        }




        [Fact]
        public async Task GetFavoriteByIdClientAndByIdProperty_Should__return_FavoriteProperty_when_Exist()
        {



            //arrange
            var context = new RealStateContext(dbContextOptions);
            var repo = new FavoritePropertyRepoitory(context);


            (var propertyInDatabase, var _userInDatabase) = await CreateProperty();


            var favoriteProperty = new FavoriteProperty()
            {

                Id = 0,
                ClientId = _userInDatabase.FirstOrDefault(s => s.FirstName == "Kodak")!.Id,
                PropertyId = propertyInDatabase.Id,

            };


            //Act
            var savefavoriteProperty = await repo.AddAsync(favoriteProperty);
            var result = await repo.GetFavoriteByIdClientAndByIdProperty(savefavoriteProperty.ClientId, savefavoriteProperty.PropertyId);

            //Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.ClientId.Should().Be(savefavoriteProperty.ClientId);

        }




        [Fact]
        public async Task GetFavoriteByIdClientAndByIdProperty_Should__return_null_when__Not_Exist()
        {



            //arrange
            var context = new RealStateContext(dbContextOptions);
            var repo = new FavoritePropertyRepoitory(context);


            (var propertyInDatabase, var _userInDatabase) = await CreateProperty();


            var FakefavoriteProperty = new FavoriteProperty()
            {

                Id = 0,
                ClientId = _userInDatabase.FirstOrDefault(s => s.FirstName == "Kodak")!.Id,
                PropertyId = propertyInDatabase.Id,

            };

            //Act
            var result = await repo.GetFavoriteByIdClientAndByIdProperty(FakefavoriteProperty.ClientId, FakefavoriteProperty.PropertyId);


            //Assert
            result.Should().BeNull();


        }



        [Fact]
        public async Task GetListFavoriteByIdClient_Should__return_FavoriteProperty_when__Exist()
        {


            //arrange
            var context = new RealStateContext(dbContextOptions);
            var repo = new FavoritePropertyRepoitory(context);


            (var propertyInDatabase, var _userInDatabase) = await CreateProperty();


            var favoriteProperty = new List<FavoriteProperty>
            {

              new()
              { 
                  Id = 0,
                  ClientId = _userInDatabase.FirstOrDefault(s => s.FirstName == "Kodak")!.Id,
                  PropertyId = propertyInDatabase.Id
              },
              new()
              {

                  Id = 0,
                  ClientId =  _userInDatabase.FirstOrDefault(s => s.FirstName == "Kodak")!.Id,
                 PropertyId = propertyInDatabase.Id
              },
              new()
              {

                  Id = 0,
                  ClientId =  _userInDatabase.FirstOrDefault(s => s.FirstName == "Kodak")!.Id,
                  PropertyId = propertyInDatabase.Id

              }

            };


            //Act
            await repo.AddRangeAsync(favoriteProperty);
            var result = await repo.GetListFavoriteByIdClient(_userInDatabase.FirstOrDefault(s => s.FirstName == "Kodak")!.Id);


            //Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().Contain(favoriteProperty);
            result.Should().HaveCountGreaterThan(1);

        }



        [Fact]
        public async Task GetListFavoriteByIdClient_Should__return_Empty_when_Not_Exist()
        {

            //arrange
            var context = new RealStateContext(dbContextOptions);
            var repo = new FavoritePropertyRepoitory(context);


            (var propertyInDatabase, var _userInDatabase) = await CreateProperty();


            //Act
            var result = await repo.GetListFavoriteByIdClient(_userInDatabase.FirstOrDefault(s => s.FirstName == "Kodak")!.Id);


            //Assert
            result.Should().BeEmpty();
         
        }


    }
}
