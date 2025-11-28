using AutoMapper;
using Humanizer;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc;
using RealStateApp.Core.Application.Dtos.SaleType;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Application.ViewModels.SaleType;


namespace RealStateApp.WebApp.Controllers
{
    public class SaleTypesController : Controller
    {
        private readonly ISaleTypeService _service;
        private readonly IMapper _mapper;

        public SaleTypesController(ISaleTypeService service,IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }


        public async Task<IActionResult> Index()
        {
            var list = await _service.GetAllListWithInclude(new List<string> { "Properties" });
            var vms= _mapper.Map<List<SaleTypeViewModel>>(list);
            return View(vms);
        }


        public IActionResult Create()
        {
            return View(new SaleTypeViewModel() { Description="", Name=""});
        }


        [HttpPost]
        public async Task<IActionResult> Create(SaleTypeViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var dto = _mapper.Map<SaleTypeDto>(vm);
            await _service.AddAsync(dto);
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            var vm= _mapper.Map<SaleTypeViewModel>(dto);
            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(SaleTypeViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);
            var dto = _mapper.Map<SaleTypeDto>(vm);


            await _service.UpdateAsync(dto.Id,dto);
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var vm = await _service.GetByIdAsync(id);
            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
