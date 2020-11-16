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

        public string OrderNo { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string Item { get; set; }
        public string MaterialCode { get; set; }
        public string Specification { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public string Unit { get; set; }
        public string UnitPrice { get; set; }
        public string TotalPrice { get; set; }
        public DateTime? LeadTime { get; set; }
        public Guid? Creator { get; set; }
        public DateTime? CreationTime { get; set; }
    }
}
