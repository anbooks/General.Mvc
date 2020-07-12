using System;
using System.Collections.Generic;
using System.Text;

namespace General.Framework.Security.Admin
{
   public  interface IAdminAuthService
    {

        void signIn(string token, string name);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Entities.SysUser getCurrentUser();
    }
}
