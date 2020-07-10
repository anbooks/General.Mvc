using General.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Framework.Security.Admin
{
    public class AuthenticationService:IAuthenticationService
    {
        public SysUser getCurrentUser()
        {
            return new SysUser { Id = Guid.NewGuid(), Name = "李四" };
        }
    }
}
