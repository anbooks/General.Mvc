using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace General.Entities
{
    [Table("Project")]
    public partial class Project
    {
        public int Id { get; set; }

        public string ProjectCode { get; set; }//订单号
        public string Describe { get; set; }//附件
        
    }
}
