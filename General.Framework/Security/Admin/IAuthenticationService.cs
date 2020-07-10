using System;
using System.Collections.Generic;
using System.Text;

namespace General.Framework.Security.Admin
{
   public  interface IAuthenticationService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Entities.SysUser getCurrentUser();
    }
}
