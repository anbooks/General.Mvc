using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Function("资料库", true, "menu-icon fa fa-pencil-square-o")]
    public class CodingLibraryController : AdminPermissionController
    {
        // GET: StyleLibrary
        
        public IActionResult Index()
        {

            return View();
        }

       
    }
}