using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Services.SOLID.O.Interfaces;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Web.Controllers.SOLID
{
    public class VillaSolidController : Controller
    {
        private readonly IVillaService _villaServiceCorrectSolid;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public VillaSolidController(IWebHostEnvironment webHostEnvironment, IVillaService villaServiceCorrectSolid)
        {
            _webHostEnvironment = webHostEnvironment;
            _villaServiceCorrectSolid = villaServiceCorrectSolid;
        }

        public IActionResult Index()
        {
            List<Villa> villasSolid = _villaServiceCorrectSolid.GetAll();
            return View(villasSolid);
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
                _villaServiceCorrectSolid.Create(villa, _webHostEnvironment.WebRootPath);

                TempData["success"] = "Villa Created Successfully";
                return RedirectToAction("Index");
            }
            return View(villa);
        }

        public IActionResult Update(int villaId)
        {
            Villa? villa = _villaServiceCorrectSolid.GetById(villaId);

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
                _villaServiceCorrectSolid.Update(villa, _webHostEnvironment.WebRootPath);

                TempData["success"] = "Villa Updated Successfully";
                return RedirectToAction("Index");
            }
            return View(villa);
        }

        public IActionResult Delete(int villaId)
        {
            Villa? villa = _villaServiceCorrectSolid.GetById(villaId);

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
            _villaServiceCorrectSolid.Delete(obj, path);
            return View(obj);
        }
    }
}
