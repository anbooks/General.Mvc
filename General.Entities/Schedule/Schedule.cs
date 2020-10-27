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
       
         public string InvoiceNo { get; set; }//编号
         public string PurchasingDocuments { get; set; }//发货人
         public string MaterielNo { get; set; }//合同号
         public string ShortTxt { get; set; }//贸易条款
         public string OrderUnit { get; set; }//货物类型
          public string PurchaseOrderQuantity { get; set; }//发票金额
         public string NetPrice { get; set; }//发票币种
         public string NetOrder { get; set; }//件数(箱数)
         public string BatchNo { get; set; }//毛重
        public string Waybill { get; set; }//实际提货/收货日期
        public int MainId { get; set; }
        public Guid Creator { get; set; }

        public DateTime CreationTime { get; set; }

        public Guid? Modifier { get; set; }

         public DateTime? ModifiedTime { get; set; }
         public bool IsDeleted { get; set; }
       
    }
}
