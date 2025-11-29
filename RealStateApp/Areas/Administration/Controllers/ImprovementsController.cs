using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Core.Application.Dtos.Improvement;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Application.ViewModels.Improvement;

namespace RealStateApp.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Authorize(Roles = "ADMIN")]
    public class ImprovementsController : Controller
    {
        private readonly IImpromentService _improvementService;
        private readonly IMapper _mapper;

        public ImprovementsController(IImpromentService improvementService, IMapper mapper)
        {
            _improvementService = improvementService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _improvementService.GetAllList();
            var vms = _mapper.Map<List<ImprovementViewModel>>(list);
            return View(vms);
        }

        // Crear
        public IActionResult Create()
        {
            return View("Save", new ImprovementViewModel() { Id = 0, Name = "", Description = "" });
        }

        [HttpPost]
        public async Task<IActionResult> Create(ImprovementViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("Save", vm);

            var dto = _mapper.Map<ImprovementDto>(vm);
            await _improvementService.AddAsync(dto);
            return RedirectToAction("Index");
        }

        // Editar
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _improvementService.GetByIdAsync(id);
            var vm = _mapper.Map<ImprovementViewModel>(dto);
            return View("Save", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ImprovementViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("Save", vm);

            var dto = _mapper.Map<ImprovementDto>(vm);
            await _improvementService.UpdateAsync(vm.Id ?? 0, dto);
            return RedirectToAction("Index");
        }

        // Confirmar eliminación
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var dto = await _improvementService.GetByIdAsync(id);
            var vm = _mapper.Map<ImprovementViewModel>(dto);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _improvementService.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
