using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace General.Entities
{
    [Table("SysUserLoginLog")]
    public class SysUserLoginLog
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string IpAddress { get; set; }

        public DateTime LoginTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Message { get; set; }

        [ForeignKey("UserId")]
        public virtual SysUser SysUser { get; set; }
    }


}
