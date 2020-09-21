using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace General.Entities
{
    [Table("Setting")]
    public partial class Setting
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }
    }
}
