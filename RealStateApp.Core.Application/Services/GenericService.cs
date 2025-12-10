using AutoMapper;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Domain.Interfaces;


namespace RealStateApp.Core.Application.Services
{
    public class GenericService <Entity, EntityDto> : IGenericService<Entity, EntityDto> where Entity : class where EntityDto : class
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Entity> _repo;

        public GenericService(IGenericRepository<Entity> repo, IMapper mapper)
        {
            _mapper=mapper;
            _repo=repo;
        }



        public virtual async Task<EntityDto?> AddAsync(EntityDto ?entityDto)
        {
            
               if(entityDto == null)  return null;
                var entity = _mapper.Map<Entity>(entityDto);
                await _repo.AddAsync(entity);
                var dto = _mapper.Map<Entity, EntityDto>(entity);
                return dto;
            
           
        }




        public virtual async Task DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);

            if (entity == null)
                throw new ApiException("Entity not found with this id");

            await _repo.DeleteAsync(id);
        }




        public async Task<EntityDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);

            var dto = _mapper.Map<EntityDto>(entity);
            return dto;
        }




        public async Task DeleteRangeAsync(List<EntityDto> entityDtos)
        {
              var entities = _mapper.Map<List<Entity>>(entityDtos);
              await  _repo.DeleteRangeAsync(entities);
            

           
        }



        public async Task<List<EntityDto>?> GetAllList()
        {
            var list = await _repo.GetAllList() ?? [];
            var dtos = _mapper.Map<List<EntityDto>>(list);
            return dtos;
        }

        public async Task<List<EntityDto>?> GetAllListWithInclude(List<string> properties)
        {
            var list = await _repo.GetAllListWithInclude(properties) ?? [];
            var dtos = _mapper.Map<List<EntityDto>>(list);
            return dtos;
        }

        

        public async Task<EntityDto?> UpdateAsync(int id, EntityDto entityDto)
        {
            var entity = _mapper.Map<Entity>(entityDto);

            var entry = await _repo.UpdateAsync(id, entity);

            var dto = _mapper.Map<EntityDto>(entry);
            return dto;
        }



        public async  Task  UpdateRangeAsync(List<EntityDto> entityDtos)
        {
            var entities = _mapper.Map<List<Entity>>(entityDtos);
            await _repo.UpdateRangeAsync(entities);
        }
    }
}
