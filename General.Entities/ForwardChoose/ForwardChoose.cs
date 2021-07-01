using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace General.Entities
{
    [Table("ForwardChoose")]
    public partial class ForwardChoose
    {
        public int Id { get; set; }

        public string Forward { get; set; }//订单号
        public string Project { get; set; }//项目
      
        public string Dest { get; set; }//附件
        public string Transport { get; set; }//IsDelet
      
        
    }
}
