using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace General.Entities
{
    [Table("Inspection")]
    public partial class Inspection
    {
        public int Id { get; set; }

        public string ContractNo { get; set; }//订单号
        public string Supplier { get; set; }//附件
        public string Manufacturer { get; set; }//附件
        public string CofC { get; set; }//附件
        public string Description { get; set; }//附件
        public string MaterialCode { get; set; }//附件
        public string Type { get; set; }//附件
        public string Size { get; set; }//附件
        public string Batch { get; set; }//附件
        public DateTime? ReceivedDate { get; set; }//附件
        public string Specification { get; set; }//附件
        public int? Qty { get; set; }//附件
        public int? ReceivedQty { get; set; }//附件
        public int? UnReceivedQty { get; set; }//附件
        public string Creator { get; set; }//
        public DateTime? CreationTime { get; set; }//
        public string Modifier { get; set; }
        public DateTime? ModifiedTime { get; set; }
    }
}
