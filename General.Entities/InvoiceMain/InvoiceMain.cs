using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace General.Entities
{
    [Table("InvoiceMain")]
    public partial class InvoiceMain
    {
        public InvoiceMain()
        {
            this.invoiceMain = new HashSet<Invoice>();

        }
        public int Id { get; set; }
        public string InvoiceMainId { get; set; }
        public string DateId { get; set; }
        public string Status { get; set; }
        public string JhyName { get; set; }
        public int Serial { get; set; }
        public bool? IsDeleted { get; set; }
        public string Creator { get; set; }//[JhyName]
        public int? flag { get; set; }
        public DateTime? CreationTime { get; set; }//
        public virtual ICollection<Invoice> invoiceMain { get; set; }
    }
}
