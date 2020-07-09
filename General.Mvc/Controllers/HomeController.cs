using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using General.Mvc.Models;
using General.Entities;
using General.Services.Category;
using General.Core;
using General.Core.Data;


namespace General.Mvc.Controllers
{ 
    public class HomeController : Controller
    {

        //private GeneralDbContext _generalDbContext;

        //public HomeController(GeneralDbContext generalDbContext)
        //{
        //    this._generalDbContext = generalDbContext;
        //}

        private ICategoryService _categoryService;

        //public HomeController(ICategoryService categoryService)
        //{
        //    this._categoryService = categoryService;
        //}




        public IActionResult Index()
        {
            //var list= _generalDbContext.Categories.ToList();
            //var list= _categoryService.getAll();
            _categoryService = EnginContext.Current.Resolve<ICategoryService>();
            var list = _categoryService.getAll();
            return View();
        }
         

        //public IActionResult Contact()
        //{
        //    ViewData["Message"] = "Your contact page.";

        //    return View();
        //}

        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
