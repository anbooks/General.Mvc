using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace General.Entities
{
    [Table("ExportTransportation")]
    public class ExportTransportation
    {
         public int Id { get; set; }//数据库建表主键必须命名为Id
       
         public string DeliverySituation { get; set; }//发运情况选择
         public string PaymentMethod { get; set; }//付款方式
         public string ImportItem { get; set; }//原进口报关单
         public bool? F_DeliverySituation { get; set; }//申请标签
         public Guid ?Applier { get; set; }//申请人
          public DateTime? ApplyTime { get; set; }//申请时间
         public string ItemNo { get; set; }//发票币种
         public string PackingList { get; set; }//件数(箱数)
         public string CreateList { get; set; }//毛重

        public Guid?Creator { get; set; }

        public DateTime?CreationTime { get; set; }

      
         public bool?IsDeleted { get; set; }
        public string Project { get; set; }
        public string OfGoods { get; set; }
        public string TotalNo { get; set; }
        public string TotalGw { get; set; }
        public string TradeTerms { get; set; }
        public string Carrier { get; set; }
        public string PickDriver { get; set; }
        public bool? F_Item { get; set; }
        public string DriverCard { get; set; }
        public string LicensePlateNo { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public string NuclearNote { get; set; }
        public string DivisionLabor { get; set; }
        public bool? GenerationClose { get; set; }
        public bool? F_LicensePlate { get; set; }
        public DateTime? LicensePlateTime { get; set; }
        public Guid? LicensePlater { get; set; }
        public bool? F_DivisionLabor { get; set; }
        public Guid? DivisionLaborer { get; set; }
        public DateTime? DivisionLaborTime { get; set; }
        public string CustomsDeclaration { get; set; }
        public bool? F_CustomsDeclaration { get; set; }
        public Guid? CustomsDeclarationer { get; set; }
        public DateTime? CustomsDeclarationTime { get; set; }
        public DateTime? ExportDeclarationTime { get; set; }
        public DateTime? CuttingLoadTime { get; set; }
        public bool? Booking { get; set; }
        public bool? F_CargoStatus { get; set; }
        public DateTime? PortDate { get; set; }
        public string TranMode { get; set; }
        public string Awb { get; set; }
        public string ShipmentPort { get; set; }
        public string DestinationPort { get; set; }
        public DateTime? DepartureDate { get; set; }
        public DateTime? EstimatedArrivalDate { get; set; }
        public DateTime? RealArrivalDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string LetThem { get; set; }

    }
}
