using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WhiteLagoon.Application.Services.Interfaces;
using WhiteLagoon.Application.Services.SOLID.L.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers.SOLID
{
    public class AmenitySolidController : Controller
    {
        private readonly Application.Services.SOLID.L.Interfaces.IAmenityService _amenityCorrectSolidService;
        private readonly IVillaService _villaService;
        public AmenitySolidController(IVillaService villaService, Application.Services.SOLID.L.Interfaces.IAmenityService amenityCorrectSolidService)
        {
            _villaService = villaService;
            _amenityCorrectSolidService = amenityCorrectSolidService;
        }
        public IActionResult Index()
        {
            List<Amenity> AmenityList = _amenityCorrectSolidService.GetAll(includeProperties: "Villa").OrderBy(u => u.Villa.Name).ToList();
            return View(AmenityList);
        }

        public IActionResult Create()
        {
            AmenityVM AmenityVM = new()
            {
                VillaList = _villaService.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };
            return View(AmenityVM);
        }

        [HttpPost]
        public IActionResult Create(AmenityVM AmenityVM)
        {
            //Remove some validations
            ModelState.Remove("Amenity.Villa");

            if (ModelState.IsValid)
            {
                _amenityCorrectSolidService.Create(AmenityVM?.Amenity);

                TempData["success"] = "Amenity Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(AmenityVM);
        }

        public IActionResult Update(int amenityId)
        {
            AmenityVM AmenityVM = new()
            {
                VillaList = _villaService.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Amenity = _amenityCorrectSolidService.Get(amenityId)
            };
            if (AmenityVM.Amenity == null)
            {
                return RedirectToAction("error", "home");
            }
            return View(AmenityVM);
        }

        [HttpPost]
        public IActionResult Update(AmenityVM AmenityVM)
        {
            ModelState.Remove("Amenity.Villa");
            if (ModelState.IsValid)
            {
                _amenityCorrectSolidService.Update(AmenityVM?.Amenity);

                TempData["success"] = "Amenity Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(AmenityVM);
        }

        public IActionResult Delete(int amenityId)
        {
            AmenityVM AmenityVM = new()
            {
                VillaList = _amenityCorrectSolidService.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Amenity = _amenityCorrectSolidService.Get(amenityId)
            };
            if (AmenityVM.Amenity == null)
            {
                return RedirectToAction("error", "home");
            }
            return View(AmenityVM);
        }

        [HttpPost]
        public IActionResult Delete(AmenityVM AmenityVM)
        {
            Amenity? objFromDb = _amenityCorrectSolidService.Get(AmenityVM?.Amenity?.Id ?? 0);
            if (objFromDb != null)
            {
                _amenityCorrectSolidService.Delete(objFromDb);

                TempData["success"] = "Amenity Deleted Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(AmenityVM);
        }
    }
}
