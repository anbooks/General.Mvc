using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace General.Entities
{
    [Table("InspectionRecord")]
    public partial class InspectionRecord
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
        public double? Qty { get; set; }//附件
        public double? PlaceQty { get; set; }//下达数量
        public double? UnPlaceQty { get; set; }//可下达数量
        public double? AcceptQty { get; set; }//接收数量 
        public string Status { get; set; }
        public string Creator { get; set; }//
        public DateTime? CreationTime { get; set; }//
        public string Modifier { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public int? InspectionId { get; set; }
    }
}
