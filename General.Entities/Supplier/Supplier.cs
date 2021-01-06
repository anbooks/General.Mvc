using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace General.Entities
{
    [Table("Supplier")]
    public partial class Supplier
    {
        public int Id { get; set; }

        public string SupplierCode { get; set; }//订单号
        public string Describe { get; set; }//附件
       
    }
}
