using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Entities
{
    public class GeneralDbContext: DbContext
    {
        public GeneralDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<SysUser> SysUsers { get; set; }
        public DbSet<SysUserToken> SysUserTokenes { get; set; }
        public DbSet<SysUserLoginLog> SysUserLoginLogs { get; set; }
        public DbSet<SysUserRole> SysUserRoles { get; set; }
        public DbSet<SysPermission> SysPermissions { get; set; }

    }
}
