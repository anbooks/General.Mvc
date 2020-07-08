using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace General.Entities
{
    [Table("SysUser")]
    [Serializable]
    public partial class SysUser
    {
        public SysUser()
        {
            SysUserTokens = new HashSet<SysUserToken>();
            SysUserLoginLogs = new HashSet<SysUserLoginLog>();
            SysUserRoles = new HashSet<SysUserRole>();
        }

        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage ="请输入账号，支持5~18位数字、字母组合")]
        [RegularExpression("^[1-9a-zA-Z]{5,18}$",ErrorMessage ="5~18数字、字母组合")]
        public string Account { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage ="请输入真实姓名")]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        [RegularExpression(@"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$",ErrorMessage = "邮箱格式错误")]
        public string Email { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        [RegularExpression(@"^1[345678]\d{9}$",ErrorMessage ="请输入11位手机号")]
        public string MobilePhone { get; set; }

        public string Password { get; set; }

        public string Salt { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [StringLength(2)]
        public string Sex { get; set; }

        public bool Enabled { get; set; }

        public bool IsAdmin { get; set; }

        public DateTime CreationTime { get; set; }

        public int LoginFailedNum { get; set; }

        public DateTime? AllowLoginTime { get; set; }

        public bool LoginLock { get; set; }

        public DateTime? LastLoginTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [StringLength(50)]
        public string LastIpAddress { get; set; }

        public DateTime? LastActivityTime { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedTime { get; set; }

        public DateTime? ModifiedTime { get; set; }

        public Guid? Modifier { get; set; }

        public Guid? Creator { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public byte[] Avatar { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual ICollection<SysUserRole> SysUserRoles { get; set; }
         
        public virtual ICollection<SysUserToken> SysUserTokens { get; set; }

        public virtual ICollection<SysUserLoginLog> SysUserLoginLogs { get; set; }
    }
}
