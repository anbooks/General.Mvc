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
       
         public string InvoiceNo { get; set; }
         public string PurchasingDocuments { get; set; }
         public string MaterielNo { get; set; }
         public string ShortTxt { get; set; }
         public string OrderUnit { get; set; }
          public string PurchaseOrderQuantity { get; set; }
         public string NetPrice { get; set; }
         public string NetOrder { get; set; }
         public string BatchNo { get; set; }
        public string Waybill { get; set; }
        public int MainId { get; set; }
        public Guid Creator { get; set; }

        public DateTime CreationTime { get; set; }

        public Guid? Modifier { get; set; }

         public DateTime? ModifiedTime { get; set; }
         public bool IsDeleted { get; set; }
       
    }
}
