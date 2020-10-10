using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/opReceiveRecordingsInput")]
    public class OPReceiveRecordingsInputController : AdminPermissionController
    {
        [Route("", Name = "opReceiveRecordingsInput")]
        [Function("填写回料清单", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.OutwardProcessingController", Sort = 1)]

        public IActionResult OPReceiveRecordingsInputIndex()
        {
            return View();
        }
    }
}