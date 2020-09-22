using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Ordinary;
using General.Framework.Menu;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Ordinary.Controllers
{
    [Function("普通用户主列表", true, "menu-icon fa fa-pencil-square-o")]
    public class CodingLibraryController : OrdinaryPermissionController
    {
        // GET: StyleLibrary
        
        public IActionResult Index()
        {

            return View();
        }

       
    }
}