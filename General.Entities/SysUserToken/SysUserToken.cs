using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace General.Entities
{
    [Serializable]  //可序列化，能转成二进制
    [Table("SysUserToken")]
    public partial class SysUserToken
    {
        public Guid Id { get; set; }

        public Guid SysUserId { get; set; }

        public DateTime ExpireTime { get; set; }

        [ForeignKey("SysUserId")]
        public virtual SysUser SysUser { get; set; }
    }
}
