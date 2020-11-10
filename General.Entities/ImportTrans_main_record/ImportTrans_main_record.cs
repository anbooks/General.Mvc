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
       
         public string Itemno { get; set; }//编号
         public string Shipper { get; set; }//发货人
         public string PoNo { get; set; }//合同号
        public string Buyer { get; set; }//采购员
        public string Incoterms { get; set; }//贸易条款
         public string CargoType { get; set; }//货物类型
          public string Invamou { get; set; }//发票金额
         public string Invcurr { get; set; }//发票币种
         public string Pcs { get; set; }//件数(箱数)
         public string Gw { get; set; }//毛重
        public DateTime? RealReceivingDate { get; set; }//实际提货/收货日期
        public Guid Creator { get; set; }

        public DateTime CreationTime { get; set; }

        public Guid? Modifier { get; set; }

         public DateTime? ModifiedTime { get; set; }
         public bool IsDeleted { get; set; }
        public Guid? Requester { get; set; }
        public DateTime? RequestTime { get; set; }
        [DataType(DataType.Date)]
        public DateTime? RequestedArrivalTime { get; set; }
        public bool? F_ShipmentCreate { get; set; }
        public string ShippingMode { get; set; }
        public bool? F_ArrivalTimeRequested { get; set; }
        public bool? F_ShippingModeGiven { get; set; }
        public DateTime? ShippingModeGivenTime { get; set; }

        public Guid? ShippingModeGiver { get; set; }
        public bool? F_DeliveryStatusInput { get; set; }
        public Guid? DeliveryStatusInputer { get; set; }
        public DateTime? DeliveryStatusInputTime { get; set; }
        public string Status { get; set; }
        public string FlighVessel { get; set; }
        public string Origin { get; set; }
        public string Dest { get; set; }
        public string Mbl { get; set; }
        public string Hbl { get; set; }
        public string Measurement { get; set; }
        public string Atd { get; set; }
        public string Ata { get; set; }
        public string Forwarder { get; set; }
        public string InventoryNo { get; set; }
        public bool? F_InventoryInput { get; set; }
        public Guid? InventoryInputer { get; set; }
        public DateTime? InventoryInputTime { get; set; }
        public bool? F_CustomsBrokerSelect { get; set; }
        public Guid? CustomsBrokerSelecter { get; set; }
        public DateTime? CustomsBrokerSelectTime { get; set; }
        //public string InventoryNo { get; set; }
        public bool? F_PortCustomerBrokerInput { get; set; }
        public Guid? PortCustomerBrokerInputer { get; set; }
        public DateTime? PortCustomerBrokerInputTime { get; set; }
        public DateTime? BlDate { get; set; }
        public DateTime? DeclarationDate { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string CustomsDeclarationNo { get; set; }        
        public string InspectionLotNo { get; set; }
        public bool? IsNeedSecondCheck { get; set; }     
        public Guid? DeliveryDateRequirer { get; set; }
        public DateTime? DeliveryDateRequiredTime { get; set; }        
        public bool? F_DeliveryDateRequired { get; set; }
        public DateTime? DeliveryRequiredDate { get; set; }
        public string DeliveryReceipt { get; set; }
        public string ChooseDelivery { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
        public string BrokenRecord { get; set; }
        public bool? F_DeliveryReceipt { get; set; }
        public Guid? DeliveryReceipter { get; set; }
        public DateTime? DeliveryReceiptTime { get; set; }
        public bool? CheckAndPass { get; set; }
        public Guid? CheckAndPassor { get; set; }
        public DateTime? CheckAndPassTime { get; set; }
        public bool? F_CheckAndPass { get; set; }
        public string ReceiptForm { get; set; }
        public string Note { get; set; }
        public Guid? ReceiptFormer { get; set; }
        public DateTime? ReceiptFormTime { get; set; }
        public bool? F_ReceiptForm { get; set; }
        public bool? SecondCheck { get; set; }
        public bool? F_SecondCheck { get; set; }
        public Guid? SecondCheckor { get; set; }
        public DateTime? SecondCheckTime { get; set; }
        public bool? Declaration { get; set; }
        public bool? F_Declaration { get; set; }
        public Guid? Declarationer { get; set; }
        public DateTime? DeclarationTime { get; set; }
    }
}
