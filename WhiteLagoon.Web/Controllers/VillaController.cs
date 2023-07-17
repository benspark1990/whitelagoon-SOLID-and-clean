using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Services.Interfaces;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Web.Controllers
{
    public class VillaController : Controller
    {
        private readonly IVillaService _villaService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public VillaController(IWebHostEnvironment webHostEnvironment, IVillaService villaService)
        {
            _webHostEnvironment = webHostEnvironment;
            _villaService = villaService;
        }

        public IActionResult Index()
        {
            List<Villa> villaList = _villaService.GetAll();
            return View(villaList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Villa villa)
        {
            if (villa.Name == villa.Description?.ToString())
            {
                ModelState.AddModelError("name", "The DisplayOrder cannot exactly match the Name.");
                TempData["error"] = "Error encountered";
            }
            if (ModelState.IsValid)
            {
                _villaService.Create(villa, _webHostEnvironment.WebRootPath);

                TempData["success"] = "Villa Created Successfully";
                return RedirectToAction("Index");
            }
            return View(villa);
        }

        public IActionResult Update(int villaId)
        {
            Villa? villa = _villaService.GetById(villaId);

            if (villa is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(villa);
        }

        [HttpPost]
        public IActionResult Update(Villa villa)
        {
            if (ModelState.IsValid && villa.Id > 0)
            {
                _villaService.Update(villa, _webHostEnvironment.WebRootPath);

                TempData["success"] = "Villa Updated Successfully";
                return RedirectToAction("Index");
            }
            return View(villa);
        }

        public IActionResult Delete(int villaId)
        {
            Villa? villa = _villaService.GetById(villaId);

            if (villa is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(villa);
        }

        [HttpPost]
        public IActionResult Delete(Villa obj)
        {
            var path = _webHostEnvironment.WebRootPath;
            _villaService.Delete(obj, path);
            return View(obj);
        }
    }
}
