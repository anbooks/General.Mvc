using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace General.Entities
{
    [Table("Attachment")]
    public partial class Attachment
    {
        public int Id { get; set; }

        public string AttachmentLoad { get; set; }//订单号
        public string Name { get; set; }//附件
        public string Type { get; set; }//附件
        public int? ImportId { get; set; }//附件
        public string Creator { get; set; }//
        public DateTime? CreationTime { get; set; }//
        
    }
}
