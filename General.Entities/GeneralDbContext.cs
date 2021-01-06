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
        public DbSet<Project> Project { get; set; }
        public DbSet<Inspection> Inspection { get; set; }
        public DbSet<InspectionRecord> InspectionRecord { get; set; }

        public DbSet<Material> Material { get; set; }
        public DbSet<ProcurementPlan> ProcurementPlan { get; set; }
        public DbSet<MaterialAcceptance> MaterialAcceptance { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SysUser> SysUsers { get; set; }
        public DbSet<SysUserToken> SysUserTokenes { get; set; }
        public DbSet<SysUserLoginLog> SysUserLoginLogs { get; set; }
        public DbSet<SysUserRole> SysUserRoles { get; set; }
        public DbSet<SysPermission> SysPermissions { get; set; }
        public DbSet<SysCustomizedList> SysCustomizedList { get; set; }
        //用到的表都要在这里注册
        public DbSet<Setting> Settings { get; set; }

        public DbSet<Schedule> Schedules { get; set; }

        public DbSet<ImportTrans_main_record> ImportTrans_main_record { get; set; }
        public DbSet<ExportTransportation> ExportTransportation { get; set; }
        public DbSet<test_JqGrid> test_JqGrid { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderMain> OrderMain { get; set; }
        public DbSet<ProcurementPlanMain> ProcurementPlanMain { get; set; }
        public DbSet<Supplier> Supplier { get; set; }
    }
}
