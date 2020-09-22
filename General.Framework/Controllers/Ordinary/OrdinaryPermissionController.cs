using General.Framework.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Framework.Controllers.Ordinary
{
    /// <summary>
    /// 需要权限验证的控制器 继承
    /// </summary>
    [PermissionActionFiltero]
    public class OrdinaryPermissionController : PublicOrdinaryController
    {

    }
}
