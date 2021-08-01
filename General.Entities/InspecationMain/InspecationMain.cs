using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace General.Entities
{
    [Table("InspecationMain")]
    public partial class InspecationMain
    {
        public InspecationMain()
        {
            this.inspecationMain = new HashSet<Inspection>();

        }
        public int Id { get; set; }
        public string InspecationMainId { get; set; }
        public string DateId { get; set; }
        public string Status { get; set; }
        public string JhyName { get; set; }
        public string Waybill { get; set; }
        public string OrderNo { get; set; }
        public string Remark { get; set; }

        public int Serial { get; set; }
        public bool? IsDeleted { get; set; }
        public string Creator { get; set; }//[JhyName]
        public int? flag { get; set; }
        public DateTime? CreationTime { get; set; }//JhTime
        public DateTime? JhTime { get; set; }
        public int FytmId { get; set; }
        public virtual ICollection<Inspection> inspecationMain { get; set; }
    }
}
