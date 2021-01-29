using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace General.Entities
{
    [Table("ProcurementPlanMain")]
    public partial class ProcurementPlanMain
    {
        public ProcurementPlanMain()
        {
            this.Plan = new HashSet<ProcurementPlan>();

        }
        [Key]
        public int Id { get; set; }

        public string PlanItem { get; set; }
        public string Prepare { get; set; }
        
        public string Project { get; set; }
        public bool IsDeleted { get; set; }
        public string Creator { get; set; }//
        public DateTime? CreationTime { get; set; }//
        public Guid? Modifier { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public virtual ICollection<ProcurementPlan> Plan { get; set; }

        //  public Guid? Creator { get; set; }
        // public DateTime? CreationTime { get; set; }
    }
}
