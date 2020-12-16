using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace General.Entities
{
    [Table("MaterialAcceptance")]
    public partial class MaterialAcceptance
    {
        public int Id { get; set; }

        public string ContractNo { get; set; }
        public string Supplier { get; set; }
        
        public string Manufacturer { get; set; }
        public string CNo { get; set; }
        public string Description { get; set; }
        public string MaterialCode { get; set; }
        public string TypeNo { get; set; }
        public string Size { get; set; }
        public string BatchNo { get; set; }
        public DateTime? DateReceived { get; set; }
        public string Specification { get; set; }
        public string TestReport { get; set; }
        public string ReceivedQty { get; set; }
        public string RejectedQty { get; set; }
        public DateTime? ManufatureDate { get; set; }
        public DateTime? InvalidDate { get; set; }
        public Guid? Creator { get; set; }
        public DateTime? CreationTime { get; set; }
    }
}
