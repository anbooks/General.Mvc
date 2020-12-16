using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace General.Entities
{
    [Table("ProcurementPlan")]
    public partial class ProcurementPlan
    {
        public int Id { get; set; }

        public string PlanItem { get; set; }
        public string Prepare { get; set; }
        
        public string Project { get; set; }
      
        public string Item { get; set; }
        public string Materialno { get; set; }
        public string PartNo { get; set; }
        public string Name { get; set; }
        public string Technical { get; set; }
        public string Size { get; set; }
        public string Width { get; set; }
        public string Length { get; set; }
        public string PlanNo { get; set; }
        public string PlanUnit { get; set; }
        public string PlanOrderNo { get; set; }
        public string PlanOrderUnit { get; set; }
        public DateTime? RequiredDockDate { get; set; }
        public string ACCovers { get; set; }
        public string Purchasing { get; set; }
        public string Application { get; set; }
        public string Remark1 { get; set; }
        public string Remark2 { get; set; }
       
      //  public Guid? Creator { get; set; }
       // public DateTime? CreationTime { get; set; }
    }
}
