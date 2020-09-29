using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace General.Entities
{
    [Table("SysCustomizedList")]
    public partial class SysCustomizedList
    {
        public Guid Id { get; set; }

        public string CustomizedClassify { get; set; }
        public string CustomizedValue { get; set; }
        public string Description { get; set; }
        public int Sort { get; set; }
        public Guid Creator { get; set; }

        public DateTime CreationTime { get; set; }

        public Guid? Modifier { get; set; }

        public DateTime? ModifiedTime { get; set; }
        public bool IsDeleted { get; set; }


    }
}
