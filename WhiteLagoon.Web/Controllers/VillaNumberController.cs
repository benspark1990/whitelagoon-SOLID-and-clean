﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly ApplicationDbContext _context;
        public VillaNumberController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int villaId)
        {
            List<VillaNumber> villaNumberList = _context.VillaNumbers.Include(u => u.Villa).ToList();
            return View(villaNumberList);
        }
        public IActionResult Create()
        {

            VillaNumberVM villaNumberVM = new()
            {
                VillaList = _context.Villas.ToList().Select(u => new SelectListItem
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
            bool isNumberUnique = _context.VillaNumbers
                .Where(u => u.Villa_Number == villaNumberVM.VillaNumber.Villa_Number).Count() == 0;

            if (ModelState.IsValid && isNumberUnique)
            {
                _context.VillaNumbers.Add(villaNumberVM.VillaNumber);
                _context.SaveChanges();
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
                VillaList = _context.Villas.ToList().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                VillaNumber = _context.VillaNumbers.FirstOrDefault(u => u.Villa_Number == villaId)
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
            ModelState.Remove("VillaNumber.Villa");
            if (ModelState.IsValid)
            {
                _context.VillaNumbers.Update(villaNumberVM.VillaNumber);
                _context.SaveChanges();
                TempData["success"] = "Villa Number Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(villaNumberVM);
        }

        public IActionResult Delete(int villaId)
        {
            VillaNumberVM villaNumberVM = new()
            {
                VillaList = _context.Villas.ToList().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                VillaNumber = _context.VillaNumbers.FirstOrDefault(u => u.Villa_Number == villaId)
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
                VillaNumber? objFromDb = _context.VillaNumbers.FirstOrDefault(x => x.Villa_Number == villaNumberVM.VillaNumber.Villa_Number);
                if (objFromDb != null)
                {
                    _context.VillaNumbers.Remove(objFromDb);
                    _context.SaveChanges();
                    TempData["success"] = "Villa Number Deleted Successfully";
                    return RedirectToAction(nameof(Index));
                }
            return View(villaNumberVM);
        }
    }
}
