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

        //private ICategoryService _categoryService;

        //public HomeController(ICategoryService categoryService)
        //{
        //    this._categoryService = categoryService;
        //}

        //private IRepository<Category> _categoryRepository;
        //private IRepository<SysUser> _sysUserRepository;
        //private IRepository<SysUser> _userRepository;

        //public HomeController(IRepository<Category> categoryRepository,IRepository<SysUser> sysUserRepository, IRepository<SysUser> userRepository)
        //{
        //    this._categoryRepository = categoryRepository;
        //    this._sysUserRepository = sysUserRepository;
        //    this._userRepository = userRepository;
        //}

        //------------------------------------------------------------------------------
        //构造方法集的形式
        //-------------------------------------------------------------------------
        private ICategoryService _categoryService;

        public HomeController(ICategoryService categoryService)
        {
            this._categoryService = categoryService;
        }


        public IActionResult Index()
        {
            //var list= _generalDbContext.Categories.ToList();
            //var list= _categoryService.getAll();
            //_categoryService = EnginContext.Current.Resolve<ICategoryService>();
            //var list = _categoryService.getAll();

            //bool b = Object.ReferenceEquals(_categoryRepository.DbContext,_sysUserRepository.DbContext);
            //bool s = Object.ReferenceEquals(_userRepository.DbContext, _sysUserRepository.DbContext);    //同一请求区域相同
            //  b= 
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
