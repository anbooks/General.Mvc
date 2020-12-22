using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace General.Entities
{
    [Table("Material")]
    public partial class Material
    {
        public int Id { get; set; }

        public string MaterialCode { get; set; }//订单号
        public string Describe { get; set; }//附件
       
    }
}
