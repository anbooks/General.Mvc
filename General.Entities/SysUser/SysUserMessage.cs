using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace General.Entities
{
    [Table("SysUserMessage")]
    [Serializable]
    public partial class SysUserMessage
    {
        public SysUserMessage()
        {
           
        }

       

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


        /// <summary>
        /// 
        /// </summary>
      
        /// <summary>
        /// 
        /// </summary>
   
    }
}
