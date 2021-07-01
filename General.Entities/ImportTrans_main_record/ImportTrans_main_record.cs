using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace General.Entities
{
    [Table("ImportTrans_main_record")]
    public class ImportTrans_main_record
    {
         public int Id { get; set; }//数据库建表主键必须命名为Id
      //  [Required(ErrorMessage = "请输入订单号")]
        public string OrderNo { get; set; }//订单号
        public string Itemno { get; set; }//编号
         public string Shipper { get; set; }//发货人
         public string PoNo { get; set; }//订单号
        public string Buyer { get; set; }//采购员
        public string Incoterms { get; set; }//贸易条款
         public string CargoType { get; set; }//货物类型
          public string Invamou { get; set; }//发票金额
         public string Invcurr { get; set; }//发票币种
         public string Pcs { get; set; }//件数(箱数)
         public string Gw { get; set; }//毛重
        public string Kind { get; set; }//类别
        public DateTime? RealReceivingDate { get; set; }//实际提货/收货日期
        public string Transportation { get; set; }
        public Guid Creator { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid? Modifier { get; set; }
         public DateTime? ModifiedTime { get; set; }
         public bool IsDeleted { get; set; }
      
        public DateTime? RequestedArrivalTime{ get; set; }
        
        public string ShippingMode { get; set; }//运输方式
        public string InventoryAttachment { get; set; }//箱单发票附件
        
        public bool? F_ShippingModeGiven { get; set; }
        
        public string Status { get; set; }//运输状态
        public string FlighVessel { get; set; }//航班号/船名
        public string Origin { get; set; }//起运港
        public string Dest { get; set; }//目的港
        public string Mbl { get; set; }//主单号
        public string MblAttachment { get; set; }//主运单附件
        public string Hbl { get; set; }//分单号
        public string HblAttachment { get; set; }//分单附件
        public string Measurement { get; set; }//计费重量
        public string MeasurementUnit { get; set; }//计费重量单位
        public DateTime? Atd { get; set; }//
        public DateTime? Ata { get; set; }//
        public string Forwarder { get; set; }//口岸报关行
        public string InventoryNo { get; set; }//核注清单号
        
        public DateTime? BlDate { get; set; }//提单日期
        public DateTime? DeclarationDate { get; set; }//申报日期
        public DateTime? ReleaseDate { get; set; }//放行日期
        public string CustomsDeclarationNo { get; set; }// 报关单号       
        public string InspectionLotNo { get; set; }//报检单号
        public bool? IsNeedSecondCheck { get; set; } //  是否生成二检  
       
        public string DeliveryRequiredDate { get; set; }//送货要求
        public string DeliveryReceipt { get; set; }//
        public string ChooseDelivery { get; set; }//选择自行送货或外部提货
        public DateTime? ActualDeliveryDate { get; set; }//实际提/送货日期
        public string BrokenRecord { get; set; }//破损记录
       
        public string CheckAndPass { get; set; }//是否核放
        
        public string ReceiptForm { get; set; }//
        public string Note { get; set; }//
        public DateTime? CheckPassTime { get; set; }
        public bool? SecondCheck { get; set; }
       
        public bool? Declaration { get; set; }
       
    }
}
