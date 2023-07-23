using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Services.Interfaces;
using WhiteLagoon.Application.Services.SOLID.O.Interfaces;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Web.Controllers
{
    public class VillaController : Controller
    {
        private readonly Application.Services.SOLID.O.Interfaces.IVillaService _villaServiceSolid;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public VillaController(IWebHostEnvironment webHostEnvironment, Application.Services.SOLID.O.Interfaces.IVillaService villaServiceSolid)
        {
            _webHostEnvironment = webHostEnvironment;
            _villaServiceSolid = villaServiceSolid;
        }

        public IActionResult Index()
        {
            List<Villa> villasSolid = _villaServiceSolid.GetAll();
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
                _villaServiceSolid.Create(villa, _webHostEnvironment.WebRootPath);

                TempData["success"] = "Villa Created Successfully";
                return RedirectToAction("Index");
            }
            return View(villa);
        }

        public IActionResult Update(int villaId)
        {
            Villa? villa = _villaServiceSolid.GetById(villaId);

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
                _villaServiceSolid.Update(villa, _webHostEnvironment.WebRootPath);

                TempData["success"] = "Villa Updated Successfully";
                return RedirectToAction("Index");
            }
            return View(villa);
        }

        public IActionResult Delete(int villaId)
        {
            Villa? villa = _villaServiceSolid.GetById(villaId);

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
            _villaServiceSolid.Delete(obj, path);
            return View(obj);
        }
    }
}
