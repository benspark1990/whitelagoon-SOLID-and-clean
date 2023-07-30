using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WhiteLagoon.Application.Services.Interfaces;
using WhiteLagoon.Application.Services.SOLID.O.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers.SOLID
{
    public class VillaNumberSolidController : Controller
    {
        private readonly IVillaService _villaService;
        private readonly Application.Services.SOLID.O.Interfaces.IVillaNumberService _villaNumberCorrectSolidService;
     
        public VillaNumberSolidController(Application.Services.SOLID.O.Interfaces.IVillaNumberService villaNumberService, IVillaService villaService)
        {
            _villaNumberCorrectSolidService = villaNumberService;
            _villaService = villaService;
        }
        public IActionResult Index(int villaId)
        {
            List<VillaNumber> villaNumberList = _villaNumberCorrectSolidService.GetAllVillaNumber(includeProperties: "Villa").OrderBy(u => u.Villa.Name).ToList();
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
            bool isNumberUnique = _villaNumberCorrectSolidService.GetVillaNumberByNumber(villaNumberVM.VillaNumber.Villa_Number).Count() == 0;

            if (ModelState.IsValid && isNumberUnique)
            {
                _villaNumberCorrectSolidService.AddVillaNumber(villaNumberVM.VillaNumber);
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
                VillaNumber = _villaNumberCorrectSolidService.GetVillaNumber(villaId)
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
                _villaNumberCorrectSolidService.UpdateVillaNumber(villaNumberVM?.VillaNumber);

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
                VillaNumber = _villaNumberCorrectSolidService.GetVillaNumber(villaId)
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
            VillaNumber? objFromDb = _villaNumberCorrectSolidService.GetVillaNumber(villaNumberVM?.VillaNumber?.Villa_Number ?? 0);
            if (objFromDb != null)
            {
                _villaNumberCorrectSolidService.RemoveVillaNumber(objFromDb);

                TempData["success"] = "Villa Number Deleted Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(villaNumberVM);
        }
    }
}
