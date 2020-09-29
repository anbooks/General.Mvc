using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace General.Entities
{
    public partial class ModifyModel
    {
  
            [UIHint("password")]
            [Display(Name = "原密码")]
            [Required]
            public string OriginalPassword { get; set; }

            [Required]
            [Display(Name = "新密码")]
            [UIHint("password")]
            public string ModifiedPassword { get; set; }

            [Required]
            [Display(Name = "确认密码")]
            [UIHint("password")]
            [Compare("ModifiedPassword", ErrorMessage = "两次密码不匹配")]
            public string ConfirmedPassword { get; set; }
        
    }
}