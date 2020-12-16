using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace General.Entities
{
    [Table("Order")]
    public partial class Order
    {
        public int Id { get; set; }

        public string OrderNo { get; set; }//订单号
        public string Attachment { get; set; }//附件
        public string PlanItem { get; set; }//索引号
        public string SupplierCode { get; set; }//供应商代码
        public string SupplierName { get; set; }//供应商名称
        public string Item { get; set; }//行号
        public string MaterialCode { get; set; }//物料代码
        public string Specification { get; set; }//材料规范
        public string Name { get; set; }//名称
        public string Size { get; set; }//规格
        public string Unit { get; set; }//单位
        public string UnitPrice { get; set; }//单价
        public string TotalPrice { get; set; }//总价
        public DateTime? LeadTime { get; set; }//
        public Guid? Creator { get; set; }//
        public DateTime? CreationTime { get; set; }//
    }
}
