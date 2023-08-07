using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WhiteLagoon.Application.Services.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Service.Models.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly IVillaService _villaService;
        private readonly IVillaNumberService _villaNumberService;
        public VillaNumberController(IVillaNumberService villaNumberService, IVillaService villaService)
        {
            _villaNumberService = villaNumberService;
            _villaService = villaService;
        }
        public IActionResult Index(int villaId)
        {
            List<VillaNumber> villaNumberList = _villaNumberService.GetAll(includeProperties: "Villa").OrderBy(u => u.Villa.Name).ToList();
            return View(villaNumberList);
            
        }

        public IActionResult Create()
        {
            VillaNumberVM villaNumberVM = new()
            {
                VillaList = _villaService.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };
            return View(villaNumberVM);
        }

        [HttpPost]
        public IActionResult Create(VillaNumberVM villaNumberVM)
        {
            //Remove some validations
            ModelState.Remove("VillaNumber.Villa");
            bool isNumberUnique = _villaNumberService.GetByNumber(villaNumberVM.VillaNumber.Villa_Number).Count() == 0;

            if (ModelState.IsValid && isNumberUnique)
            {
                _villaNumberService.Create(villaNumberVM.VillaNumber);
                TempData["success"] = "Villa Number Successfully";
                return RedirectToAction(nameof(Index));
            }
            if (!isNumberUnique)
            {
                TempData["error"] = "Villa number already exists. Please use a different villa number.";
            }
            return View(villaNumberVM);
        }

        public IActionResult Update(int villaId)
        {
            VillaNumberVM villaNumberVM = new()
            {
                VillaList = _villaService.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                VillaNumber = _villaNumberService.Get(villaId)
            };
            if (villaNumberVM.VillaNumber == null)
            {
                return RedirectToAction("error", "home");
            }
            return View(villaNumberVM);
        }

        [HttpPost]
        public IActionResult Update(VillaNumberVM villaNumberVM)
        {
            if (ModelState.IsValid)
            {
                _villaNumberService.Update(villaNumberVM?.VillaNumber);

                TempData["success"] = "Villa Number Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(villaNumberVM);
        }

        public IActionResult Delete(int villaId)
        {
            VillaNumberVM villaNumberVM = new()
            {
                VillaList = _villaService.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                VillaNumber = _villaNumberService.Get(villaId)
            };
            if (villaNumberVM.VillaNumber == null)
            {
                return RedirectToAction("error", "home");
            }
            return View(villaNumberVM);
        }

        [HttpPost]
        public IActionResult Delete(VillaNumberVM villaNumberVM)
        {
            VillaNumber? objFromDb = _villaNumberService.Get(villaNumberVM?.VillaNumber?.Villa_Number ?? 0);
            if (objFromDb != null)
            {
                _villaNumberService.Delete(objFromDb);

                TempData["success"] = "Villa Number Deleted Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(villaNumberVM);
        }
    }
}
