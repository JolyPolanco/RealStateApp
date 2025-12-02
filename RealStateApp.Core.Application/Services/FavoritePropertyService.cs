using AutoMapper;
using RealStateApp.Core.Application.Dtos.FavoriteProperty;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Domain.Entities;
using RealStateApp.Core.Domain.Interfaces;


namespace RealStateApp.Core.Application.Services
{
    public class FavoritePropertyService : GenericService<FavoriteProperty, CreateFavoritePropertyDto>, IFavoritePropertyService
    {


        private readonly IFavoritePropertyRepository favoritePropertyRepository;
        private readonly IMapper mapper;




        public FavoritePropertyService(IFavoritePropertyRepository repo, IMapper mapper) : base(repo, mapper)
        {

            this.favoritePropertyRepository = repo;
            this.mapper = mapper;


        }




        public override async Task<CreateFavoritePropertyDto?> AddAsync(CreateFavoritePropertyDto entityDto)
        {
            try
            {


                var FavoriteProperty = await favoritePropertyRepository.GetFavoriteByIdClientAndByIdProperty(entityDto.ClientId,entityDto.PropertyId);


                if (FavoriteProperty  == null)
                {


                    var entity = mapper.Map<FavoriteProperty>(entityDto);
                    var favoriteProperty =  await favoritePropertyRepository.AddAsync(entity);
                    var dto = mapper.Map<CreateFavoritePropertyDto>(favoriteProperty);
                    return dto;
                }

                return null;
              
            }
            catch(Exception ex)
            {

                return default;

            }
        }






        public override async Task DeleteAsync(int id)
        {
            try
            {

                var entity = await favoritePropertyRepository.GetByIdAsync(id);

                if(entity != null)
                {

                    await favoritePropertyRepository.DeleteAsync(entity.Id); 

                }

            }
            catch(Exception ex)
            {


                throw new Exception(ex.Message); 

            }
        }




    }
}
