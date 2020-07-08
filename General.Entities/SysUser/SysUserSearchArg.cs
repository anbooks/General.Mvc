using System;
using System.Collections.Generic;
using System.Text;

namespace General.Entities
{
    /// <summary>
    /// 查询参数
    /// </summary>
    public class SysUserSearchArg
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string q { get; set; }

        /// <summary>
        /// 冻结、启用
        /// </summary>
        public bool? enabled { get; set; }

        /// <summary>
        /// 是否登录锁
        /// </summary>
        public bool? unlock { get; set; }

        /// <summary>
        /// 操作员
        /// </summary>
        public Guid? suid { get; set; }

        /// <summary>
        /// 角色id
        /// </summary>
        public Guid? roleId { get; set; }
    }
}
