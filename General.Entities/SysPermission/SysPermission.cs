using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace General.Entities
{

    [Table("SysPermission")]
    public partial class SysPermission
    {
        public Guid Id { get; set; }

        public int CategoryId { get; set; }

        public Guid RoleId { get; set; }

        public Guid Creator { get; set; }

        public DateTime CreationTime { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        [ForeignKey("RoleId")]
        public virtual SysRole SysRole { get; set; }

        [ForeignKey("Creator")]
        public virtual SysUser SysUser { get; set; }
    }
}
