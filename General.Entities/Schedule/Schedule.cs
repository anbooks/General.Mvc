using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace General.Entities
{
    [Table("Schedule")]
    public class Schedule
    {
         public int Id { get; set; }//数据库建表主键必须命名为Id

        public string Buyer { get; set; }//采购员
        public string OrderNo { get; set; }//订单号
        public string OrderLine { get; set; }//订单行号
        public string ReferenceNo { get; set; }//索引号
        public string MaterialCode { get; set; }//物料代码
        public string Description { get; set; }//品名
        public string Type { get; set; }//型号
        public string Specification { get; set; }//规范
        public string Thickness { get; set; }//厚度
        public string Length { get; set; }//长
        public string Width { get; set; }//宽  
        public string PartNo { get; set; }//宽
        public string PurchaseQuantity { get; set; }//采购数量
        public string PurchaseUnit { get; set; }//采购单位
        public string UnitPrice { get; set; }//单价
        public string TotalPrice { get; set; }//总价
        public DateTime? ShipmentDate { get; set; }//发运日期
        public string Consignor { get; set; }//发货人
        public string Manufacturers { get; set; }//制造商
        public string OriginCountry { get; set; }//原产国
        public string BatchNo { get; set; }//炉批号（质量编号）
        public string Waybill { get; set; }//运单号
        public string Package { get; set; }//包装规格
        public string Books { get; set; }//账册号
        public string BooksItem { get; set; }//账册项号
        public string RecordUnit { get; set; }//备案单位
        public string RecordUnitReducedPrice { get; set; }//按备案单位折算单价
        public string LegalUnits { get; set; }//法定单位
        public string LegalUniteReducedPrice { get; set; }//按法定单位折算单价
        public string Qualification { get; set; }//合格证号
        public string CodeNo { get; set; }//打码号
        public double? PlanNo { get; set; }//计划数量
        public double? Reduced { get; set; }//折算关系
        public string ReducedNo { get; set; }//折算数量
        public string InvoiceNo { get; set; }//发票号
        public string PlanUnit { get; set; }
        
        public int MainId { get; set; }
        public Guid Creator { get; set; }

        public DateTime CreationTime { get; set; }

        public Guid? Modifier { get; set; }

         public DateTime? ModifiedTime { get; set; }
         public bool IsDeleted { get; set; }
        public bool Sjflag { get; set; }
    }
}
