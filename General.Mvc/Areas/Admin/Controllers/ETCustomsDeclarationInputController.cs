using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/etCustomsDeclarationInput")]
    public class ETCustomsDeclarationInputController : AdminPermissionController
    {
        [Route("", Name = "etCustomsDeclarationInput")]
        [Function("填写报关单", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ExportTransportationController", Sort = 1)]

        public IActionResult ETCustomsDeclarationInputIndex()
        {
            return View();
        }
    }
}