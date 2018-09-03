using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BusinessLayer;
using DataLayer;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace DMS.Controllers
{
    [Authorize(Roles="Admin,User")]
    public class CategoryController : Controller
    {
        private CategoryService _cats;
        public CategoryController(DMSContext db)
        {
            _cats = new CategoryService(db);
        }

        public IActionResult Index()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            var categories = _cats.GetAll(email);
            return View(categories);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category cat)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            var status = _cats.CreateCategory(cat,email);
            if (status)
            {
                ViewBag.success = "Created successfully";
            }
            else
            {
                ViewBag.error = "Something was wrong.";
            }
            return View();
        }

     
        public IActionResult Delete(int id)
        {
            var status = _cats.DeleteCategory(id);
            if (status)
            {
                TempData["success"] = "Deleted successfully";
            }
            else
            {
                TempData["Error"] = "Error Occurred";
            }
          return RedirectToAction("Index");
        }
    }
}