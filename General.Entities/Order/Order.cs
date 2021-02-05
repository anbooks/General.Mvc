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
        public string Unit { get; set; }//订单单位
        public string UnitPrice { get; set; }//单价
        public string TotalPrice { get; set; }//总价
        public DateTime? LeadTime { get; set; }//
        public Guid? Creator { get; set; }//
        public DateTime? CreationTime { get; set; }//
        public Guid? Modifier { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public DateTime? OrderConfirmDate { get; set; }//订单确认日期
        public string OrderSigner { get; set; }//订单签订人
        public string SignerCard { get; set; }//签订人胸卡号 string TotalPrice { get; set; }//总价
        public string TradeTerms { get; set; }//贸易条款
        public string Transport { get; set; }//运输代理
        public string Project { get; set; }//项目
        public string MaterialCategory { get; set; }//物料类别
        public string PartNo { get; set; }//牌号
        public string Width { get; set; }//
        public string Length { get; set; }//
        public string Package { get; set; }//包装规格
        public string Qty { get; set; }//订单数量
        public string Currency { get; set; }//币种
        public string Manufacturer { get; set; }//制造商
        public string Origin { get; set; }//原产国
        public double? PlanUnit { get; set; }//计划单位
        public double? Reduced { get; set; }//折算单位
        public int MainId { get; set; }
        public bool IsDeleted { get; set; }
        // public ProcurementPlanMain PlanMain { get; set; }
        [ForeignKey("MainId")]
        public virtual OrderMain Main { get; set; }
    }
}
