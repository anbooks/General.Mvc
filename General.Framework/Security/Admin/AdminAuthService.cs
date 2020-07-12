using General.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace General.Framework.Security.Admin
{
    public class AdminAuthService:IAdminAuthService
    {
        public IHttpContextAccessor _httpContextAccessor;

        public AdminAuthService(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }

        public SysUser getCurrentUser()
        {
            return new SysUser { Id = Guid.NewGuid(), Name = "李四" };
        }

        public void signIn(string token, string name)
        {
            //throw new NotImplementedException();
            ClaimsIdentity claimsIdentity = new ClaimsIdentity("Forms");
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Sid, token));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, name));
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            _httpContextAccessor.HttpContext.SignInAsync("General", claimsPrincipal);

        }
    }
}
