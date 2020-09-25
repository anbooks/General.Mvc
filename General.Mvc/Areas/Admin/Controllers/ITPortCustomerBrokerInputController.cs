using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/itPortCustomerBrokerInput")]
    public class ITPortCustomerBrokerInputController : AdminPermissionController
    {
        [Route("", Name = "itPortCustomerBrokerInput")]
        [Function("口岸报关行信息填报", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ImportTransportationController", Sort = 8)]

        public IActionResult ITPortCustomerBrokerInputIndex()
        {
            return View();
        }
    }
}