using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace General.Entities
{
    [Table("OrderMain")]
    public partial class OrderMain
    {
        public OrderMain()
        {
            this.OrderImport = new HashSet<Order>();

        }
        public int Id { get; set; }

        public string OrderNo { get; set; }//订单号
       
        public string SupplierCode { get; set; }//供应商代码
        public string SupplierName { get; set; }//供应商名称
        public DateTime? OrderConfirmDate { get; set; }//订单确认日期
        public string OrderSigner { get; set; }//订单签订人
        public string SignerCard { get; set; }//签订人胸卡号 string TotalPrice { get; set; }//总价
        public string TradeTerms { get; set; }//贸易条款
        public string Transport { get; set; }//运输代理
        public string Project { get; set; }//项目
        public string Buyer { get; set; }//项目
        public string MaterialCategory { get; set; }//物料类别
        public bool IsDeleted { get; set; }
        public Guid? Creator { get; set; }//
        public DateTime? CreationTime { get; set; }//
        public Guid? Modifier { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public virtual ICollection<Order> OrderImport { get; set; }
    }
}
