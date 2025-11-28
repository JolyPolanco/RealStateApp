using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Core.Application.Dtos.Properties;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Application.ViewModels.Properties;

namespace RealStateApp.Areas.Administration.Controllers
{
    [Area("Administration")]
    public class PropertyTypesController : Controller
    {
        private readonly IPropertyTypeService _propertyTypeService;
        private readonly IMapper _mapper;

        public PropertyTypesController(IPropertyTypeService propertyTypeService, IMapper mapper)
        {
            _propertyTypeService = propertyTypeService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var types = _propertyTypeService.GetAllListWithInclude(new List<string> { "Properties" });
            var vms = _mapper.Map<List<PropertyTypeViewModel>>(types);
            return View(vms);
        }

        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var entity = await _propertyTypeService.GetByIdAsync(id);
            var vm = _mapper.Map<PropertyTypeViewModel>(entity);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _propertyTypeService.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        public IActionResult Create()
        {
            return View("Save", new SavePropertyTypeViewModel() { Description="", Name=""});
        }

        [HttpPost]
        public async Task<IActionResult> Create(SavePropertyTypeViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("Save", vm);

            var dto = _mapper.Map<PropertyTypeDto>(vm);
            await _propertyTypeService.AddAsync(dto);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var entity = await _propertyTypeService.GetByIdAsync(id);
            var vm = _mapper.Map<SavePropertyTypeViewModel>(entity);
            return View("Save", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SavePropertyTypeViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("Save", vm);

            var dto = _mapper.Map<PropertyTypeDto>(vm);
            await _propertyTypeService.UpdateAsync(vm.Id ?? 0, dto);
            return RedirectToAction("Index");
        }
    }
}
