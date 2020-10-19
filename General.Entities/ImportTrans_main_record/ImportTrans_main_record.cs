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
        public DateTime? RequestedArrivalTime { get; set; }
        public bool? ShipmentCreateflag { get; set; }
    }
}
