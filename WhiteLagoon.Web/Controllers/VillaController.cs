using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Services.Interfaces;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Web.Controllers
{
    public class VillaController : Controller
    {
        private readonly IVillaService _villaServiceNotSolid;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public VillaController(IWebHostEnvironment webHostEnvironment, IVillaService villaServiceSolid)
        {
            _webHostEnvironment = webHostEnvironment;
            _villaServiceNotSolid = villaServiceSolid;
        }

        public IActionResult Index()
        {
            List<Villa> villasSolid = _villaServiceNotSolid.GetAll();
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
                _villaServiceNotSolid.Create(villa, _webHostEnvironment.WebRootPath);

                TempData["success"] = "Villa Created Successfully";
                return RedirectToAction("Index");
            }
            return View(villa);
        }

        public IActionResult Update(int villaId)
        {
            Villa? villa = _villaServiceNotSolid.GetById(villaId);

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
                _villaServiceNotSolid.Update(villa, _webHostEnvironment.WebRootPath);

                TempData["success"] = "Villa Updated Successfully";
                return RedirectToAction("Index");
            }
            return View(villa);
        }

        public IActionResult Delete(int villaId)
        {
            Villa? villa = _villaServiceNotSolid.GetById(villaId);

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
            _villaServiceNotSolid.Delete(obj, path);
            return View(obj);
        }
    }
}
