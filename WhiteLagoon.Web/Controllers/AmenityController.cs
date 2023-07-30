using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WhiteLagoon.Application.Services.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    public class AmenityController : Controller
    {
        private readonly IAmenityService _amenityNotSolidService;
        private readonly IVillaService _villaService;
        public AmenityController(IVillaService villaService, IAmenityService amenityNotSolidService)
        {
            _villaService = villaService;
            _amenityNotSolidService = amenityNotSolidService;
        }
        public IActionResult Index()
        {
            List<Amenity> AmenityList = _amenityNotSolidService.GetAll(includeProperties: "Villa").OrderBy(u => u.Villa.Name).ToList();
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
                _amenityNotSolidService.Create(AmenityVM?.Amenity);

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
                Amenity = _amenityNotSolidService.Get(amenityId)
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
                _amenityNotSolidService.Update(AmenityVM?.Amenity);

                TempData["success"] = "Amenity Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(AmenityVM);
        }

        public IActionResult Delete(int amenityId)
        {
            AmenityVM AmenityVM = new()
            {
                VillaList = _amenityNotSolidService.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Amenity = _amenityNotSolidService.Get(amenityId)
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
            Amenity? objFromDb = _amenityNotSolidService.Get(AmenityVM?.Amenity?.Id ?? 0);
            if (objFromDb != null)
            {
                _amenityNotSolidService.Delete(objFromDb);

                TempData["success"] = "Amenity Deleted Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(AmenityVM);
        }
    }
}
