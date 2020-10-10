using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/opSendRecordingsInput")]
    public class OPSendRecordingsInputController : AdminPermissionController
    {
        [Route("", Name = "opSendRecordingsInput")]
        [Function("填写发料记录", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.OutwardProcessingController", Sort = 1)]

        public IActionResult OPSendRecordingsInputIndex()
        {
            return View();
        }
    }
}