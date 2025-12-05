using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Domain.Common.Enums;
using RealStateApp.Core.Domain.Entities;
using RealStateApp.Infraestructure.Identity.Contexts;
using RealStateApp.Infraestructure.Identity.Entities;
using RealStateApp.Infraestructure.Persistence.Contexts;
using RealStateApp.Infraestructure.Persistence.Repositories;
using System.Runtime.CompilerServices;


namespace RealStateApp.Integration.Tests.Persistence.Repositories
{
    public class OfferRepositoryTests
    {

        private readonly DbContextOptions<RealStateContext> dbContextOptions;
        private readonly DbContextOptions<IdentityContext> IdentitydbContextOptions;



        public OfferRepositoryTests()
        {



            dbContextOptions = new DbContextOptionsBuilder<RealStateContext>()
              .UseInMemoryDatabase(databaseName: $"RealStateApp_{Guid.NewGuid()}")
              .Options;




            IdentitydbContextOptions = new DbContextOptionsBuilder<IdentityContext>()
               .UseInMemoryDatabase(databaseName: $"IdentityRealStateApp_{Guid.NewGuid()}")
               .Options;


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





        [Fact]
        public async Task AddAsync_Should_return_offer_when_Add_To_Database()
        {

            //Arrange
            //creamos usuarios

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

            //Act


            var offerDto = new Offer()
            {
                Id = 0,
                Amount = 1300000000,
                Date = DateTime.Now,
                ClientId = _UserInDatabase.FirstOrDefault(s => s.FirstName == "Kodak")!.Id,
                PropertyId = property.Id,
                Status = OfferStatus.Pending

            };

            var OfferRepo = new OfferRepository(context);
            var result = await OfferRepo.AddAsync(offerDto);



            //Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.ClientId.Should().Be(_UserInDatabase.FirstOrDefault(s => s.FirstName == "Kodak")!.Id);


        }



        /// <summary>
        ///  metodo que hay que revisar
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddAsync_Should_return_Throw_when_offer_is_null()
        {

            //Arrange
            var context = new RealStateContext(dbContextOptions);

            var OfferRepo = new OfferRepository(context);

            //Act

            Func<Task> act = async () => await OfferRepo.AddAsync(null!);

            //Assert
            await act.Should().ThrowAsync<ArgumentNullException>();

        }



        [Fact]
        public async Task GetByIdAsync_Should_return_offer_when_exists()
        {

            //Arrange
            //creamos usuarios
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

            //Act


            var offerDto = new Offer()
            {
                Id = 0,
                Amount = 1300000000,
                Date = DateTime.Now,
                ClientId = _UserInDatabase.FirstOrDefault(s => s.FirstName == "Kodak")!.Id,
                PropertyId = property.Id,
                Status = OfferStatus.Pending

            };

            var OfferRepo = new OfferRepository(context);
            var SaveOffer = await OfferRepo.AddAsync(offerDto);
            var result = await OfferRepo.GetByIdAsync(SaveOffer.Id);


            //Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Amount.Should().Be(SaveOffer.Amount);
            result.Id.Should().Be(SaveOffer.Id);

        }





        [Fact]
        public async Task GetByIdAsync_Should_return_null_when_not_exists()
        {

            //Arrange
            var context = new RealStateContext(dbContextOptions);

            var OfferRepo = new OfferRepository(context);
            var result = await OfferRepo.GetByIdAsync(1500);


            //Assert
            result.Should().BeNull();

        }







        [Fact]
        public async Task DeleteAsync_Should_return_Null_when_exists()
        {

            //Arrange
            //creamos usuarios
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

            //Act


            var offerDto = new Offer()
            {
                Id = 0,
                Amount = 1300000000,
                Date = DateTime.Now,
                ClientId = _UserInDatabase.FirstOrDefault(s => s.FirstName == "Kodak")!.Id,
                PropertyId = property.Id,
                Status = OfferStatus.Pending

            };

            var OfferRepo = new OfferRepository(context);
            var SaveOffer = await OfferRepo.AddAsync(offerDto);
            await OfferRepo.DeleteAsync(SaveOffer.Id);
            var result = await OfferRepo.GetByIdAsync(offerDto.Id);

            //Assert
            result.Should().BeNull();

        }





        [Fact]
        public async Task DeleteAsync_Should_return_No_Throw_when_No_found()
        {

            //Arrange
            //creamos usuarios
            var context = new RealStateContext(dbContextOptions);


            var OfferRepo = new OfferRepository(context);

            Func<Task> act = async () => await OfferRepo.GetByIdAsync(999);

            //Assert
            await act.Should().NotThrowAsync();

        }





        [Fact]
        public async Task UpdateAsync_Should_Modify_offer_InDatabase()
        {

            //Arrange
            //creamos usuarios

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

            //Act


            var offerDto = new Offer()
            {
                Id = 0,
                Amount = 1300000000,
                Date = DateTime.Now,
                ClientId = _UserInDatabase.FirstOrDefault(s => s.FirstName == "Kodak")!.Id,
                PropertyId = property.Id,
                Status = OfferStatus.Pending

            };

            var OfferRepo = new OfferRepository(context);
            var UpdateOffer = await OfferRepo.AddAsync(offerDto);
            UpdateOffer.Amount = 1400000000;
            var result = await OfferRepo.UpdateAsync(UpdateOffer.Id, UpdateOffer);


            //Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(UpdateOffer.Id);
            result.Amount.Should().Be(UpdateOffer.Amount);


        }



        [Fact]
        public async Task UpdateAsync_Should_return_null_when_offer_NoFound()
        {

            //Arrange
            //creamos usuarios
            var context = new RealStateContext(dbContextOptions);

            //Act
            var Fakeoffer = new Offer()
            {
                Id = 0,
                Amount = 1300000000,
                Date = DateTime.Now,
                ClientId = "",
                PropertyId = 0,
                Status = OfferStatus.Pending

            };

            var OfferRepo = new OfferRepository(context);
            var result = await OfferRepo.UpdateAsync(Fakeoffer.Id, Fakeoffer);


            //Assert
            result.Should().BeNull();

        }




        [Fact]
        public async Task GetAllAsync_Should_Return_All_Offers()
        {


            //Arrange
            //creamos usuarios
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

            //Act



            var offers = new List<Offer>
            {

                new Offer()
                {
                Id = 0,
                Amount = 1300000000,
                Date = DateTime.Now,
                ClientId = _UserInDatabase.FirstOrDefault(s => s.FirstName == "Kodak")!.Id,
                PropertyId = property.Id,
                Status = OfferStatus.Pending

                },
                 new Offer()
                {
                Id = 0,
                Amount = 1270000000,
                Date = DateTime.Now.AddDays(20),
                ClientId = _UserInDatabase.FirstOrDefault(s => s.FirstName == "Kodak")!.Id,
                PropertyId = property.Id,
                Status = OfferStatus.Pending

                },
                  new Offer()
                {
                Id = 0,
                Amount = 1500000000,
                Date = DateTime.Now.AddDays(40),
                ClientId = _UserInDatabase.FirstOrDefault(s => s.FirstName == "Kodak")!.Id,
                PropertyId = property.Id,
                Status = OfferStatus.Pending

                },

            };


            var OfferRepo = new OfferRepository(context);
            await OfferRepo.AddRangeAsync(offers);
            var result = await OfferRepo.GetAllList();



            //Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().NotBeEmpty();
        }



        [Fact]
        public async Task GetAllAsync_Should_Empty_when_NoOffers()
        {

            //Arrange
            //creamos usuarios

            var context = new RealStateContext(dbContextOptions);

            //Act

            var OfferRepo = new OfferRepository(context);
            var result = await OfferRepo.GetAllList();


            //Assert
            result.Should().HaveCount(0);
            result.Should().BeEmpty();
        }




        [Fact]
        public async Task GetListByIdClientAndPropertyIdAsync_should_return_All_offers_when_Exists()
        {


            //Arrange
            //creamos usuarios
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

            //Act



            var offers = new List<Offer>
            {

                new Offer()
                {
                Id = 0,
                Amount = 1300000000,
                Date = DateTime.Now,
                ClientId = _UserInDatabase.FirstOrDefault(s => s.FirstName == "Kodak")!.Id,
                PropertyId = property.Id,
                Status = OfferStatus.Pending

                },
                 new Offer()
                {
                Id = 0,
                Amount = 1270000000,
                Date = DateTime.Now.AddDays(20),
                ClientId = _UserInDatabase.FirstOrDefault(s => s.FirstName == "Kodak")!.Id,
                PropertyId = property.Id,
                Status = OfferStatus.Pending

                },
                  new Offer()
                {
                Id = 0,
                Amount = 1500000000,
                Date = DateTime.Now.AddDays(40),
                ClientId = _UserInDatabase.FirstOrDefault(s => s.FirstName == "Kodak")!.Id,
                PropertyId = property.Id,
                Status = OfferStatus.Pending

                },

            };


            var OfferRepo = new OfferRepository(context);
            await OfferRepo.AddRangeAsync(offers);
            var result = await OfferRepo.GetListByIdClientAndPropertyIdAsync(_UserInDatabase.FirstOrDefault(s => s.FirstName == "Kodak")!.Id, SaveProperty.Id);



            //Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().NotBeEmpty();
        }


        [Fact]
        public async Task GetListByIdClientAndPropertyIdAsync_should_return_Empty_when_Not_Exists()
        {


            //Arrange
            //creamos usuarios
            var context = new RealStateContext(dbContextOptions);

            //Act


            var OfferRepo = new OfferRepository(context);
            var result = await OfferRepo.GetListByIdClientAndPropertyIdAsync("39939sowowoow", 999);



            //Assert
            result.Should().HaveCount(0);
            result.Should().BeEmpty();

        }



        [Fact]
        public async Task GetListByPropertyIdAsync_should_return_All_offers_when_IdPropertyExists()
        {


            //Arrange
            //creamos usuarios
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

            //Act



            var offers = new List<Offer>
            {

                new Offer()
                {
                Id = 0,
                Amount = 1300000000,
                Date = DateTime.Now,
                ClientId = _UserInDatabase.FirstOrDefault(s => s.FirstName == "Kodak")!.Id,
                PropertyId = property.Id,
                Status = OfferStatus.Pending

                },
                 new Offer()
                {
                Id = 0,
                Amount = 1270000000,
                Date = DateTime.Now.AddDays(20),
                ClientId = _UserInDatabase.FirstOrDefault(s => s.FirstName == "Kodak")!.Id,
                PropertyId = property.Id,
                Status = OfferStatus.Pending

                },
                  new Offer()
                {
                Id = 0,
                Amount = 1500000000,
                Date = DateTime.Now.AddDays(40),
                ClientId = _UserInDatabase.FirstOrDefault(s => s.FirstName == "Kodak")!.Id,
                PropertyId = property.Id,
                Status = OfferStatus.Pending

                },

            };


            var OfferRepo = new OfferRepository(context);
            await OfferRepo.AddRangeAsync(offers);
            var result = await OfferRepo.GetListByPropertyIdAsync(SaveProperty.Id);



            //Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().NotBeEmpty();
        }



        [Fact]
        public async Task GetListByPropertyIdAsync_should_return_Empty_when_IdProperty_Not_Exists()
        {


            //Arrange
            //creamos usuarios
            var context = new RealStateContext(dbContextOptions);
           
            //Act
            var OfferRepo = new OfferRepository(context);
            var result = await OfferRepo.GetListByPropertyIdAsync(1500);


            //Assert
            result.Should().HaveCount(0);
            result.Should().BeEmpty();
        }




    }
}
