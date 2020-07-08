using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace General.Entities
{
    [Table("SysUserRole")]
    public partial class SysUserRole
    {
        public Guid Id { get; set; }

        
        public Guid RoleId { get; set; }

        public Guid UserId { get; set; }

        [ForeignKey("RoleId")]
        public virtual SysRole SysRole { get; set; }

        [ForeignKey("UserId")]
        public virtual SysUser SysUser { get; set; }
    }
}
